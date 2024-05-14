namespace PrintStatus.DAL.Repositories.Implementations;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Data;
using DOM.Models;
using DTOs;
using Helpers;
using Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Responses;

public class UserAccountRepository(IOptions<JwtSection> config, AppDbContext appDbContext) : IUserAccount
{
	public async Task<IRepositoryResponse<Register>> CreateAsync(Register user)
	{
		if (user is null) return RepositoryResponse<Register>.Failure([], "Пустой объект");

		var checkUser = await FindUserByEmail(user.Email!);
		if (checkUser is not null) return RepositoryResponse<Register>.Failure([], "Пользователь уже существует");

		var applicationUser = await AddToDatabase(new ApplicationUser()
		{
			FullName = user.FullName,
			Email = user.Email,
			Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
		});

		var checkAdminRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.Admin));
		if (checkAdminRole is null)
		{
			var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
			await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
			return RepositoryResponse<Register>.Success(user, "Аккаунт администратора создан");
		}
		var checkUserRole = await appDbContext.SystemRoles.FirstOrDefaultAsync(_ => _.Name!.Equals(Constants.User));
		SystemRole response = new();
		if (checkUserRole is null)
		{
			response = await AddToDatabase(new SystemRole() { Name = Constants.User });
			await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
		}
		else
		{
			await AddToDatabase(new UserRole() { RoleId = checkUserRole.Id, UserId = applicationUser.Id });
		}
		return RepositoryResponse<Register>.Success(user, "Пользователь создан");
	}

	public async Task<IAuthResponse> SignInAsync(Login user)
	{
		if (user is null) return AuthResponse.Failure("Пустая модель");

		var applicationUser = await FindUserByEmail(user.Email!);
		if (applicationUser is null) return AuthResponse.Failure("Пользователь не найден");

		if (!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
			return AuthResponse.Failure("Неверный логин или пароль");

		var getUserRole = await FindUserRole(applicationUser.Id);
		if (getUserRole is null) return AuthResponse.Failure("Роль пользователя не найдена");

		var getRoleName = await FindRoleName(getUserRole.RoleId);

		string jwtToken = GenerateToken(applicationUser, getRoleName.Name!);
		string refreshToken = GenerateRefreshToken();

		var findUser = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == applicationUser.Id);
		if (findUser is not null)
		{
			findUser!.Token = refreshToken;
			await appDbContext.SaveChangesAsync();
		}
		else
		{
			await AddToDatabase(new RefreshTokenInfo() { Token = refreshToken, UserId = applicationUser.Id });
		}
		return AuthResponse.Success("Успешная авторизация", jwtToken, refreshToken);
	}

	public async Task<IRepositoryResponse<ApplicationUser>> GetUserById(int id)
	{
		if (id <= 0) return RepositoryResponse<ApplicationUser>.Failure(new List<string>(),"Неверный идентификатор пользователя");
		var result = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Id == id);
		if (result is null) return RepositoryResponse<ApplicationUser>.Failure(new List<string>(), "Пользователь не найден");
		var getRole = await FindUserRole(id);
		var roleName = await FindRoleName(getRole.RoleId);
		return RepositoryResponse<ApplicationUser>.Success(result, roleName.Name!);

	}

	private string GenerateToken(ApplicationUser user, string role)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Value.Key!));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		var userClaims = new[]
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.FullName!),
			new Claim(ClaimTypes.Email, user.Email!),
			new Claim(ClaimTypes.Role, role!)
		};
		var token = new JwtSecurityToken(
			issuer: config.Value.Issuer,
			audience: config.Value.Audience,
			claims: userClaims,
			expires: DateTime.UtcNow.AddMinutes(15),
			signingCredentials: credentials
		);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	private async Task<UserRole> FindUserRole(int userId) =>
			await appDbContext.UserRoles.FirstOrDefaultAsync(r => r.UserId == userId);

	private async Task<SystemRole> FindRoleName(int roleId) =>
			await appDbContext.SystemRoles.FirstOrDefaultAsync(r => r.Id == roleId);

	private string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

	private async Task<ApplicationUser> FindUserByEmail(string email) =>
		await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Email!.ToLower()!.Equals(email!.ToLower()));

	private async Task<T> AddToDatabase<T>(T model)
	{
		var result = appDbContext.Add(model!);
		await appDbContext.SaveChangesAsync();
		return (T)result.Entity;
	}

	public async Task<IAuthResponse> RefreshTokenAsync(RefreshToken token)
	{
		if (token is null) return AuthResponse.Failure("Пустой объект");

		var findToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.Token!.Equals(token.Token));
		if (findToken is null) return AuthResponse.Failure("Не найден RefreshToken в базе данных");

		var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(_ => _.Id == findToken.UserId);
		if (user is null) return AuthResponse.Failure("Не удалось создать RefreshToken, так как пользователь не найден");

		var userRole = await FindUserRole(user.Id);
		var roleName = await FindRoleName(userRole.Id);
		string jwtToken = GenerateToken(user, roleName.Name!);
		string refreshToken = GenerateRefreshToken();

		var updateRefreshToken = await appDbContext.RefreshTokenInfos.FirstOrDefaultAsync(_ => _.UserId == user.Id);
		if (updateRefreshToken is null) return AuthResponse.Failure("Не удалось создать RefreshToken, так как пользователь не авторизован");

		updateRefreshToken.Token = refreshToken;
		await appDbContext.SaveChangesAsync();
		return AuthResponse.Success("Token refreshed successfully", jwtToken, refreshToken);
	}

	public async Task<IRepositoryResponse<List<ManageUser>>> GetUsers()
	{
		var allUsers = await GetApplicationUsers();
		var allUserRoles = await GetUserRoles();
		var allRoles = await GetSystemRoles();
		
		if(allUsers.Count == 0 || allRoles.Count == 0) return null!;
		var users = new List<ManageUser>();
		foreach(var user in allUsers)
		{
			var userRole = allUserRoles.FirstOrDefault(_ => _.UserId == user.Id);
			var roleName = allRoles.FirstOrDefault(_ => _.Id == userRole!.RoleId);
			users.Add(new ManageUser(){UserId = user.Id, Name = user.FullName!, Email = user.Email!, Role = roleName!.Name!});
		}
		return RepositoryResponse<List<ManageUser>>.Success(users);
	}

	public async Task<IRepositoryResponse<ManageUser>> UpdateUser(ManageUser user)
	{
		var getRole = (await GetSystemRoles()).FirstOrDefault(r => r.Name!.Equals(user.Role));
		var userRole = await appDbContext.UserRoles.FirstOrDefaultAsync(u => u.UserId == user.UserId);
		userRole!.RoleId = getRole!.Id;
		await appDbContext.SaveChangesAsync();
		return RepositoryResponse<ManageUser>.Success(user, "Роль пользователя обновлена");
	}

	public async Task<List<SystemRole>> GetRoles() => await GetSystemRoles();

	public async Task<IRepositoryResponse<bool>> DeleteUser(int id)
	{
		var user = await appDbContext.ApplicationUsers.FirstOrDefaultAsync(u => u.Id == id);
		appDbContext.ApplicationUsers.Remove(user!);
		await appDbContext.SaveChangesAsync();
		return RepositoryResponse<bool>.Success(true, "Пользователь удален");
	}
	
	private async Task<List<SystemRole>> GetSystemRoles() => await appDbContext.SystemRoles.AsNoTracking().ToListAsync();
	private async Task<List<UserRole>> GetUserRoles() => await appDbContext.UserRoles.AsNoTracking().ToListAsync();
	private async Task<List<ApplicationUser>> GetApplicationUsers() => await appDbContext.ApplicationUsers.AsNoTracking().ToListAsync();
}
 
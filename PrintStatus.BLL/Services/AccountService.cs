using Microsoft.AspNetCore.Identity;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Interfaces;

namespace PrintStatus.BLL;

public class AccountService(
                        UserManager<IdentityUser> userManager,
                        SignInManager<IdentityUser> signInManager,
                        RoleManager<IdentityRole> roleManager,
                        IUserProfileRepository userRepo
                        ) : IAccountService
{
	private readonly UserManager<IdentityUser> _userManager = userManager;
	private readonly SignInManager<IdentityUser> _signInManager = signInManager;
	private readonly IUserProfileRepository _userRepo = userRepo;
	private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public Task<IdentityResult> AssignRoleAsync(string userName, string roleName)
	{
		throw new NotImplementedException();
	}

	public Task<IdentityResult> DeleteUserAsync(string userId)
	{
		throw new NotImplementedException();
	}

	public async Task<IServiceResult<IEnumerable<string>>> GetRolesAsync(string userId)
	{
		if (string.IsNullOrEmpty(userId)) return ServiceResult<IEnumerable<string>>.Failure("Неверный идентификатор пользователя");
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null) return ServiceResult<IEnumerable<string>>.Failure($"Пользователь с ID = {userId} не найден");
		var userRoles = await _userManager.GetRolesAsync(user);
		var roles = new List<string>();
		foreach (var role in userRoles)
		{
			roles.Add(role);
		}
		return ServiceResult<IEnumerable<string>>.Success(roles, "Роли получены");
	}

	public async Task<AuthResult> LoginAsync(string userName, string password)
	{
		var user = await _userManager.FindByNameAsync(userName) ?? await _userManager.FindByEmailAsync(userName);
		if (user != null)
		{
			var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
			if (result.Succeeded)
			{
				var roles = await _userManager.GetRolesAsync(user);
				return new AuthResult
				{
					IsAuthenticated = true,
					Roles = roles,
					IdentityUserId = user.Id
				};
			}
		}
		return new AuthResult
		{
			IsAuthenticated = false,
			Errors = [new IdentityError { Description = "Авторизация не удалась" }]
		};
	}

	public async Task<IdentityResult> RegisterAsync(string userName, string password)
	{
		var userExists = await _userManager.FindByNameAsync(userName);
		if (userExists != null)
		{
			return IdentityResult.Failed(new IdentityError { Description = "Пользователь с таким логином уже существует" });
		}
		var newUser = new IdentityUser { UserName = userName };
		var result = await _userManager.CreateAsync(newUser, password);
		if (result.Succeeded)
		{
			await _userRepo.AddUserProfileAsync(newUser.Id);
			return result;
		}
		return IdentityResult.Failed(new IdentityError { Description = "Создание пользователя не удалось! Пожалуйста, проверьте логин и пароль и попробуйте ещё раз" });
	}

	public Task<IdentityResult> UpdatePassword(string userId, string oldPassword, string newPassword)
	{
		throw new NotImplementedException();
	}

	public Task<IdentityResult> UpdateRoleAsync(string userId, string roleName)
	{
		throw new NotImplementedException();
	}

	public Task<IdentityResult> UpdateUserAsync(string userId, string newUserName)
	{
		throw new NotImplementedException();
	}
}

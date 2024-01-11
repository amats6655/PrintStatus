using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Query;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL;

public class UserService : IUserService
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly IUserProfileRepository _userRepo;
	public UserService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUserProfileRepository userRepo)
	{
		_roleManager = roleManager;
		_userManager = userManager;
		_userRepo = userRepo;
	}
	
	public async Task<IdentityResult> AddUserAsync(string userName, string password)
	{
		var user = new IdentityUser { UserName = userName};
		var result = await _userManager.CreateAsync(user, password);
		if(result.Succeeded)
		{
			await _userRepo.AddUserProfileAsync(user.Id);
		}
		return result;
	}
	
	public async Task<IdentityResult> AssignRoleAsync(string userName, string roleName)
	{
		var user = await _userManager.FindByNameAsync(userName);
		if(user == null)
		{
			return IdentityResult.Failed(new IdentityError { Description = $"User not found: {userName}"});
		}
		
		var roleExists = await _roleManager.RoleExistsAsync(roleName);
		if(!roleExists)
		{
			await _roleManager.CreateAsync(new IdentityRole(roleName));
		}
		
		return null;
		
		// добавить роль пользователю
	}
	
	public async Task<IdentityResult> UpdateUserAsync(string userId, string newUserName)
	{
		var user = await _userManager.FindByIdAsync(userId);
		if (user == null)
		{
			return IdentityResult.Failed(new IdentityError { Description = $"user not found {userId}"});
		}
		user.UserName = newUserName;
		var result = await _userManager.UpdateAsync(user);
		return result;
	}
}

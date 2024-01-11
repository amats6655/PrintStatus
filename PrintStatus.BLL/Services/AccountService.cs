using Microsoft.AspNetCore.Identity;
using PrintStatus.DOM.Models;

namespace PrintStatus.BLL;

public class AccountService : IAccountService
{
	private readonly UserManager<IdentityUser> _userManager;
	private readonly SignInManager<IdentityUser> _signInManager;
	
	public AccountService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}
	
	public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
	{
		var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
		if(user != null)
		{
			var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
			return result.Succeeded;
		}
		return false;
	}
}

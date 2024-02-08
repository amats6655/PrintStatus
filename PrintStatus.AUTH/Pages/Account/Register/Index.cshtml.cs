using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PrintStatus.AUTH.Models;
using PrintStatus.AUTH.Pages.Register;

namespace PrintStatus.AUTH.Pages.Register;

[SecurityHeaders]
[AllowAnonymous]

public class Index : PageModel
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly SignInManager<ApplicationUser> _signInManager;
	
	[BindProperty]
	public RegisterModel RegisterModel {get; set;}
	
	public Index(
		UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager)
	{
		_userManager = userManager;
		_signInManager = signInManager;
	}
	
	
	public async Task<IActionResult> OnPost()
	{
		if(ModelState.IsValid)
		{
			var user = new ApplicationUser { UserName = RegisterModel.Email, Email = RegisterModel.Email };
			var result = await _userManager.CreateAsync(user, RegisterModel.Password);

			if (result.Succeeded)
			{
				await _signInManager.SignInAsync(user, isPersistent: false);
				// return LocalRedirect(RegisterModel.ReturnUrl);
			}
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError(string.Empty, error.Description);
				Console.WriteLine(error.Description); 
			}
		}
		// If we got this far, something failed, redisplay form
		return Page();
	}
}
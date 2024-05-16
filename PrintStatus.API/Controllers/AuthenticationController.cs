using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.DAL.DTOs;
using PrintStatus.DAL.Repositories.Interfaces;

namespace PrintStatus.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController(IUserAccount accountInterface) : ControllerBase
{
	[HttpPost("register")]
	public async Task<IActionResult> CreateAsync(Register user)
	{
		if (user is null) return BadRequest("Model is empty");
		var result = await accountInterface.CreateAsync(user);
		return Ok(result);
	}
	
	[HttpPost("login")]
	public async Task<IActionResult> SignInAsync(Login user)
	{
		if(user is null) return BadRequest("Model is empty");
		var result = await accountInterface.SignInAsync(user);
		return Ok(result);
	}
	
	[HttpPost("refresh-token")]
	public async Task<IActionResult> RefreshTokenAsync(RefreshToken token)
	{
		if (token is null) return BadRequest("Model is empty");
		var result = await accountInterface.RefreshTokenAsync(token);
		return Ok(result);
	}
	
	[HttpGet("users")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> GetUsersAsync()
	{
		var users = await accountInterface.GetUsers();
		if(users is null) return NotFound();
		return Ok(users);
	}
	
	[HttpPut("update-user")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> UpdateUser(ManageUser manageUser)
	{
		var result = await accountInterface.UpdateUser(manageUser);
		return Ok(result);
	}
	
	[HttpGet("roles")]
	public async Task<IActionResult> GetRoles()
	{
		var roles = await accountInterface.GetRoles();
		if(roles is null) return NotFound();
		return Ok(roles);
	}
	
	[HttpDelete("delete-user/{id}")]
	[Authorize(Roles = "Admin")]
	public async Task<IActionResult> DeleteUser(int id)
	{
		var result = await accountInterface.DeleteUser(id);
		return Ok(result);
	}
}

using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LocationController : ControllerBase
{
	private readonly ILocationService _locationService;

	public LocationController(ILocationService locationService)
	{
		_locationService = locationService;
	}

	[HttpPost("add")]
	public async Task<IActionResult> Add(LocationDTO model)
	{
		if (model is null) return BadRequest("Bad request made");
		return Ok(await _locationService.InsertAsync(model));
	}

	[HttpGet("all")]
	public async Task<IActionResult> GetAll()
	{
		return Ok(await _locationService.GetAllAsync());
	}

	[HttpGet("single/{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		if (id <= 0) return BadRequest("Invalid request sent");
		return Ok(await _locationService.GetByIdAsync(id));
	}

	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		if (id <= 0) return BadRequest("Invalid request sent");
		return Ok(await _locationService.DeleteAsync(id));
	}

	[HttpPut("update")]
	public async Task<IActionResult> Update(Location model)
	{
		var roles = HttpContext.User.Claims.Where(r => r.Type == ClaimTypes.Role).ToList();
		if (!roles.Any(n => n.Value == "Admin")) return Unauthorized();
		if (model is null) return BadRequest("Bad request made");
		return Ok(await _locationService.UpdateAsync(model));
	}
}
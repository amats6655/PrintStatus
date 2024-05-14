using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CalcTypeController : ControllerBase
{
	private readonly ICalcTypeService _calcTypeService;

	public CalcTypeController(ICalcTypeService calcTypeService)
	{
		_calcTypeService = calcTypeService;
	}

	[HttpPost("add")]
	public async Task<IActionResult> Add(CalcType model)
	{
		if (model is null) return BadRequest("Bad request made");
		return Ok(await _calcTypeService.InsertAsync(model));
	}

	[HttpGet("all")]
	public async Task<IActionResult> GetAll()
	{
		return Ok(await _calcTypeService.GetAllAsync());
	}

	[HttpGet("single/{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		if (id <= 0) return BadRequest("Invalid request sent");
		return Ok(await _calcTypeService.GetByIdAsync(id));
	}

	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		if (id <= 0) return BadRequest("Invalid request sent");
		return Ok(await _calcTypeService.DeleteAsync(id));
	}

	[HttpPut("update")]
	public async Task<IActionResult> Update(CalcType model)
	{
		var roles = HttpContext.User.Claims.Where(r => r.Type == ClaimTypes.Role).ToList();
		if (!roles.Any(n => n.Value == "Admin")) return Unauthorized();
		if (model is null) return BadRequest("Bad request made");
		return Ok(await _calcTypeService.UpdateAsync(model));
	}
}
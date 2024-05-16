using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsumableController : ControllerBase
{
	private readonly IConsumableService _consumableService;

	public ConsumableController(IConsumableService consumableService)
	{
		_consumableService = consumableService;
	}
	[HttpPost("add")]
	public async Task<IActionResult> Add(NewConsumableDTO model)
	{
		if (model is null) return BadRequest("Bad request made");
		return Ok(await _consumableService.InsertAsync(model));
	}
	
	[HttpGet("all")]
	public async Task<IActionResult> GetAll()
	{
		return Ok(await _consumableService.GetAllAsync());
	}

	[HttpGet("single/{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		if (id <= 0) return BadRequest("Invalid request sent");
		return Ok(await _consumableService.GetByIdAsync(id));
	}
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		if (id <= 0) return BadRequest("Invalid request sent");
		return Ok(await _consumableService.DeleteAsync(id));
	}
	
	[HttpPut ("update/{id}")]
	public async Task<IActionResult> Update(Consumable model)
	{
		if (model is null) return BadRequest("Invalid request sent");
		return Ok(await _consumableService.UpdateAsync(model));
	}
}
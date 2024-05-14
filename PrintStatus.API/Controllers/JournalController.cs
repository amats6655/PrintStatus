using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JournalController : ControllerBase
{
	private readonly IJournalService _journalService;

	public JournalController(IJournalService journalService)
	{
		_journalService = journalService;
	}
	[HttpPost("add")]
	public async Task<IActionResult> Add(Journal model)
	{
		if (model is null) return BadRequest("Bad request made");
		return Ok(await _journalService.InsertAsync(model));
	}
	
	[HttpGet("all")]
	public async Task<IActionResult> GetAll()
	{
		return Ok(await _journalService.GetAllAsync());
	}

	[HttpGet("single/{id}")]
	public async Task<IActionResult> GetById(int id)
	{
		if (id <= 0) return BadRequest("Invalid request sent");
		return Ok(await _journalService.GetByIdAsync(id));
	}
	[HttpDelete("delete/{id}")]
	public async Task<IActionResult> Delete(int id)
	{
		if (id <= 0) return BadRequest("Invalid request sent");
		return Ok(await _journalService.DeleteAsync(id));
	}
	
	[HttpPut ("update/{id}")]
	public async Task<IActionResult> Update(Journal model)
	{
		if (model is null) return BadRequest("Invalid request sent");
		return Ok(await _journalService.UpdateAsync(model));
	}
	
	[HttpGet("allByPrinter/{id}")]
	public async Task<IActionResult> GetAllByPrinter(int modelId)
	{
		if (modelId <= 0) return BadRequest("Invalid request sent");
		return Ok(await _journalService.GetAllByPrinterIdAsync(modelId));
	}
	
	[HttpGet("allByOid/{id}")]
	public async Task<IActionResult> GetAllByOid(int oidId, int modelId)
	{
		if (modelId <= 0 || oidId <= 0) return BadRequest("Invalid request sent");
		return Ok(await _journalService.GetAllByOidIdAsync(oidId, modelId));
	}
}
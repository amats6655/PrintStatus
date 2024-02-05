using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class HistoryController : ControllerBase
	{
		private readonly IHistoryManagementService _historyService;

		public HistoryController(IHistoryManagementService historyService)
		{
			_historyService = historyService;
		}

		[HttpPost]
		public async Task<IActionResult> Add(History history)
		{
			var result = await _historyService.AddHistoryAsync(history);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await _historyService.GetByIdAsync(id);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("printer/{printerId}")]
		public async Task<IActionResult> GetAllByPrinterId(int printerId)
		{
			var result = await _historyService.GetAllByPrinterIdAsync(printerId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet]
		public async Task<IActionResult> GetAllHistories()
		{
			var result = await _historyService.GetAllHistoriesAsync();
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}
	}
}
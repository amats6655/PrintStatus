using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;

namespace Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PrinterController : ControllerBase
	{
		private readonly IBasePrinterManagementService _basePrinterService;
		public PrinterController(IBasePrinterManagementService printerManagementService)
		{
			_basePrinterService = printerManagementService;
		}

		[HttpPost]
		public async Task<IActionResult> Add(NewPrinterDTO printer)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			printer.ApplicationUserId = userId;
			var result = await _basePrinterService.AddAsync(printer);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _basePrinterService.DeleteAsync(id, userId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _basePrinterService.GetAllAsync();
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await _basePrinterService.GetByIdAsync(id);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("location/{locationId}")]
		public async Task<IActionResult> GetAllByLocation(int locationId)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _basePrinterService.GetAllByLocationAsync(locationId, userId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("model/{modelId}")]
		public async Task<IActionResult> GetAllByModel(int modelId)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _basePrinterService.GetAllByModelAsync(modelId, userId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("user/{identityUserId}")]
		public async Task<IActionResult> GetAllByUser()
		{

			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _basePrinterService.GetAllByUserAsync(userId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("detail/{id}")]
		public async Task<IActionResult> GetDetailById(int id)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _basePrinterService.GetDetailByIdAsync(id, userId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}



		//TODO: Не трекается изменяемый принтер
		[HttpPut]
		public async Task<IActionResult> Update(PrinterDTO printerDTO)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _basePrinterService.UpdateAsync(printerDTO, userId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}
	}
}
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
		private readonly IBasePrinterManagementService _printService;
		public PrinterController(IBasePrinterManagementService printerManagementService)
		{
			_printService = printerManagementService;
		}

		[HttpGet]
		[Route("getAll")]
		public async Task<IActionResult> Get()
		{
			var claims = HttpContext.User.Claims;
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			Console.WriteLine(userId);
			var result = await _printService.GetAllByUserAsync(userId);
			if (!result.Data.Any())
			{
				return Ok(new List<PrinterDTO>());
			}
			return new JsonResult(result);
		}


		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _printService.GetByIdAsync(id);
			if (!result.IsSuccess)
			{
				return BadRequest(result.Message);
			}
			return Ok(result.Data);
		}

		[HttpGet()]
		[Route("getDetail")]
		public async Task<IActionResult> GetDetail(int printId)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _printService.GetDetailByIdAsync(printId, userId);
			if (!result.IsSuccess) { return BadRequest(result.Message); }
			return new JsonResult(result);
		}

		[HttpPost]
		public async Task<IActionResult> AddPrinter([FromBody] NewPrinterDTO value)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			value.ApplicationUserId = userId;
			var result = await _printService.AddAsync(value);
			if (!result.IsSuccess) return BadRequest(result.Message);
			return Ok(result.Data);
		}

		[HttpPost]
		[Route("delete")]
		public async Task<IActionResult> DeletePrinter([FromBody] int id)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _printService.DeleteAsync(id, userId);
			if (!result.IsSuccess) return BadRequest(result.Message);
			return Ok(result.Message);
		}

		[HttpPost]
		[Route("Update")]
		public async Task<IActionResult> UpdatePrinter([FromBody] PrinterDTO printerDTO)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _printService.UpdateAsync(printerDTO, userId);
			if (!result.IsSuccess) return BadRequest(result.Message);
			return new JsonResult(result);
		}
	}
}

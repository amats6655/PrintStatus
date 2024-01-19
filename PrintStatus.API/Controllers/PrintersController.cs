using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PrintStatus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PrintersController : ControllerBase

	{
		private readonly IBasePrinterManagementService _printService;
		public PrintersController(IBasePrinterManagementService printerManagementService)
		{
			_printService = printerManagementService;
		}

		[HttpGet]
		[Authorize]
		[Route("getAll")]
		public async Task<IActionResult> Get()
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var result = await _printService.GetAllByUserAsync(userId);
			if (result == null)
			{
				return Ok(new List<PrinterDTO>());
			}
			return Ok(result);
		}


		[HttpGet("{id}")]
		[Authorize]
		public async Task<IActionResult> Get(int id)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var result = await _printService.GetByIdAsync(id);
			if (!result.IsSuccess)
			{
				return BadRequest(result.Message);
			}
			return Ok(result.Data);
		}


		[HttpPost]
		public async Task<IActionResult> Post([FromBody] NewPrinterDTO value)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			value.IdentityUserId = userId;
			var result = await _printService.AddAsync(value);
			if (!result.IsSuccess) return BadRequest(result.Message);
			return Ok(result.Data);
		}

	}
}

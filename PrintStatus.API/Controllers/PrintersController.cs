using System.Security.Claims;
using System.Text.Json;
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
		// GET: api/<PrintersController>
		[HttpGet]
		public async Task<IEnumerable<PrinterDTO>> Get()
		{
			return await _printService.GetAllAsync();
		}

		// GET api/<PrintersController>/5
		[HttpGet("{id}")]
		public async Task<PrinterDTO> Get(int id)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var result = await _printService.GetByIdAsync(id, userId);
			return result;
		}

		// POST api/<PrintersController>
		[HttpPost]
		public async Task<StatusCodeResult> Post([FromBody] NewPrinterDTO value)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var result = await _printService.AddAsync(value.Title, value.IpAddress, value.LocationId, userId);
			if (result != null)
			{
				return Ok();
			}
			return NoContent();
		}

		// PUT api/<PrintersController>/5
		[HttpPut("{id}")]
		public void Put()
		{

		}

		// DELETE api/<PrintersController>/5
		[HttpDelete("{id}")]
		public async Task<bool> Delete(int id)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			return await _printService.DeleteAsync(id, userId);
		}
	}
}

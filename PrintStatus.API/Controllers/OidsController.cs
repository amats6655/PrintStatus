using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;

namespace PrintStatus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class OidsController(
			IPrintOidManagementService printOidService) : ControllerBase
	{
		private readonly IPrintOidManagementService _printOidService = printOidService;


		[HttpPost]
		public async Task<IActionResult> Add(OidDTO oid, int printModelId)
		{
			var result = await _printOidService.AddAsync(oid, printModelId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var result = await _printOidService.DeleteAsync(id);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await _printOidService.GetByIdAsync(id);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpGet("model/{modelId}")]
		public async Task<IActionResult> GetAllByModel(int modelId)
		{
			var result = await _printOidService.GetAllByModelAsync(modelId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		[HttpPut]
		public async Task<IActionResult> Update(OidDTO oid)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _printOidService.UpdateAsync(oid, userId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}
	}
}
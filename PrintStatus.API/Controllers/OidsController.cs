using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;

namespace PrintStatus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class OidsController(
			IPrintOidManagementService oidService) : ControllerBase
	{
		private readonly IPrintOidManagementService _oidService = oidService;

		[HttpGet]
		[Route("getAllByModel")]
		public async Task<IActionResult> GetAllByModel(int modelId)
		{
			var result = await _oidService.GetAllByModelAsync(modelId);
			if (!result.IsSuccess) return BadRequest(result.Message);
			return new JsonResult(result);
		}

		[HttpGet]
		[Route("getById")]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await _oidService.GetByIdAsync(id);
			if (!result.IsSuccess) return BadRequest(result?.Message);
			return new JsonResult(result);
		}

		[HttpPost]
		[Route("add")]
		public async Task<IActionResult> AddOid([FromBody] OidDTO oidDTO, int printerId)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			var resultAdd = await _oidService.AddAsync(oidDTO, printerId);
			if (!resultAdd.IsSuccess) return BadRequest(resultAdd?.Message);
			return new JsonResult(resultAdd);
		}
	}
}

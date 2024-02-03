using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL;
using PrintStatus.BLL.Interfaces;

namespace PrintStatus.API
{

	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ModelsController : ControllerBase
	{
		private readonly IPrintModelManagementService _printModelService;
		public ModelsController(IPrintModelManagementService printModelService)
		{
			_printModelService = printModelService;
		}

		[HttpGet]
		[Route("getAll")]
		public async Task<IActionResult> GetAll([FromBody] int modelId)
		{
			var result = await _printModelService.GetAllAsync();
			if (!result.IsSuccess) return BadRequest(result.Message);
			return new JsonResult(result);
		}

		[HttpPost]
		[Route("update")]
		public async Task<IActionResult> Update([FromBody] PrintModelDTO printModelDTO)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("sub"))?.Value;
			if (userId == null) return BadRequest("Неавторизованная операция");
			var result = await _printModelService.UpdateAsync(printModelDTO, userId);
			if (!result.IsSuccess) return BadRequest(result.Message);
			return new JsonResult(result);
		}
	}
}
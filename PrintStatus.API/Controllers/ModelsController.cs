using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL;
using PrintStatus.BLL.Interfaces;

namespace PrintStatus.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class PrintModelController : ControllerBase
	{
		private readonly IPrintModelManagementService _printModelService;

		public PrintModelController(IPrintModelManagementService printModelService)
		{
			_printModelService = printModelService;
		}

		/// <summary>
		/// Adds a new print model.
		/// </summary>
		/// <param name="modelTitle">The title of the print model.</param>
		/// <returns>The result of the operation.</returns>
		[HttpPost]
		public async Task<IActionResult> Add(string modelTitle)
		{
			var result = await _printModelService.AddAsync(modelTitle);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		/// <summary>
		/// Deletes a print model by ID.
		/// </summary>
		/// <param name="id">The ID of the print model to delete.</param>
		/// <returns>The result of the operation.</returns>
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var result = await _printModelService.DeleteAsync(id);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		/// <summary>
		/// Gets all print models.
		/// </summary>
		/// <returns>The result of the operation.</returns>
		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var result = await _printModelService.GetAllAsync();
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		/// <summary>
		/// Gets a print model by ID.
		/// </summary>
		/// <param name="id">The ID of the print model to get.</param>
		/// <returns>The result of the operation.</returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var result = await _printModelService.GetByIdAsync(id);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}

		/// <summary>
		/// Updates a print model.
		/// </summary>
		/// <param name="printerModelDTO">The updated print model DTO.</param>
		/// <param name="identityUserId">The identity user ID.</param>
		/// <returns>The result of the operation.</returns>
		[HttpPut]
		public async Task<IActionResult> Update(PrintModelDTO printerModelDTO)
		{
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
			var result = await _printModelService.UpdateAsync(printerModelDTO, userId);
			if (result.IsSuccess)
				return Ok(result);
			return BadRequest(result.Message);
		}
	}
}

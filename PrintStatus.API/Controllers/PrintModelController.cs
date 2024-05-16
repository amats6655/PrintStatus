using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrintModelController: ControllerBase
    {
        private readonly IPrintModelService _printModelService;

        public PrintModelController(IPrintModelService printModelService)
        {
            _printModelService = printModelService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(string modelName)
        {
            if(modelName is null) return BadRequest("Bad request made");
            return Ok(await _printModelService.InsertAsync(modelName));
        }
        
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid request sent");
            return Ok(await _printModelService.DeleteAsync(id));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _printModelService.GetAllAsync());
        }
        
        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid request sent");
            return Ok(await _printModelService.GetByIdAsync(id));
        }
        
        [HttpPut("update")]
        public async Task<IActionResult> Update(PrintModel model)
        {
            var roles = HttpContext.User.Claims.Where(r => r.Type == ClaimTypes.Role).ToList();
            if (!roles.Any(n => n.Value == "Admin")) return Unauthorized();
            if(model is null) return BadRequest("Bad request made");
            return Ok(await _printModelService.UpdateAsync(model));
        }
    }
}

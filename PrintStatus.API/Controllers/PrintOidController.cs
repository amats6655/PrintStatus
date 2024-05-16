using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrintOidController : ControllerBase
    {
        private readonly IPrintOidService _printOidService;
        public PrintOidController(IPrintOidService printOidService)
        {
            _printOidService = printOidService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(NewPrintOidDTO model)
        {
            if(model is null) return BadRequest("Bad request made");
            return Ok(await _printOidService.InsertAsync(model));
        }
        
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id <= 0) return BadRequest("Invalid request sent");
            return Ok(await _printOidService.DeleteAsync(id));
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _printOidService.GetAllAsync());
        }
        
        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetSingle(int id)
        {
            if(id <= 0) return BadRequest("Invalid request sent");
            return Ok(await _printOidService.GetByIdAsync(id));
        }
        
        [HttpPut("update")]
        public async Task<IActionResult> Update(PrintOid model)
        {
            var roles = HttpContext.User.Claims.Where(r => r.Type == ClaimTypes.Role).ToList();
            if (!roles.Any(n => n.Value == "Admin")) return Unauthorized();
            if(model is null) return BadRequest("Bad request made");
            return Ok(await _printOidService.UpdateAsync(model));
        }
    }
}

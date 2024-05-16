using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Helpers;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrinterController : ControllerBase
    {
        private readonly IPrinterService _printerService;
        public PrinterController(IPrinterService printerService)
        {
            _printerService = printerService;
        }
        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> Add(NewPrinterDTO model)
        {
            if (model is null) return BadRequest("Bad request made");
            var userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)!
                .Value);
            return Ok(await _printerService.InsertAsync(model, userId));
        }
        
        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Invalid request sent");
            var userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)!
                .Value);
            return Ok(await _printerService.DeleteAsync(id, userId));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _printerService.GetAllAsync());
        }

        [HttpGet("allByLocation/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAllByLocation(int id)
        {
            if(id <= 0) return BadRequest("Invalid request sent");
            var userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)!
                .Value);
            return Ok(await _printerService.GetAllByLocationAsync(id, userId));
        }

        [HttpGet("allByModel/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAllByModel(int id)
        {
            if(id <= 0) return BadRequest("Invalid request sent");
            var userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)!
                .Value);
            return Ok(await _printerService.GetAllByModelAsync(id, userId));
        }
        
        [HttpGet("allByUser")]
        [Authorize]
        public async Task<IActionResult> GetAllByUser()
        {
            var userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)!
                .Value);
            return Ok(await _printerService.GetAllByUserAsync(userId));
        }
        
        [HttpGet("single/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Invalid request sent");
            return Ok(await _printerService.GetByIdAsync(id));
        }

        [HttpGet("detail/{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(int id)
        {
            if (id <= 0) return BadRequest("Invalid request sent");
            var userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)!
                .Value);
            return Ok(await _printerService.GetDetailByIdAsync(id, userId));
        }
        
        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> Update(Printer model)
        {
            if (model is null) return BadRequest("Bad request made");
            var userId = int.Parse(HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)!
                .Value);
            return Ok(await _printerService.UpdateAsync(model, userId));
        }
    }
}

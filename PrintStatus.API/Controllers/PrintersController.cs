using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
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
        public IEnumerable<PrinterDTO> Get()
        {
            return new List<PrinterDTO>();
        }

        // GET api/<PrintersController>/5
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            var result = await _printService.GetByIdAsync(id);
            string json = JsonSerializer.Serialize(result);
            return json;
        }

        // POST api/<PrintersController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<PrintersController>/5
        [HttpPut("{id}")]
        public void Put()
        {

        }

        // DELETE api/<PrintersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

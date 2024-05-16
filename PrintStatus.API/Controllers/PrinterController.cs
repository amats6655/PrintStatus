using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Services.Interfaces;
using PrintStatus.DOM.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace PrintStatus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrinterController : ControllerBase
    {
        private readonly IPrinterService _printerService;
        public PrinterController(IPrinterService printerService)
        {
            _printerService = printerService;
        }

        /// <summary>
        /// Добавляет новый принтер пользователю.
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/printer/add
        ///     {
        ///        "name": "Принтер 1",
        ///        "ipAddress": "192.168.1.100",
        ///        "locationId": 1
        ///     }
        ///
        /// </remarks>
        /// <param name="model">Информация о новом принтере.</param>
        /// <returns>Информация о добавленном принтере.</returns>
        /// <response code="201">Принтер успешно добавлен</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// 
        [HttpPost("add")]
        [SwaggerResponse(200, "Принтер успешно удален у пользователя", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        [SwaggerResponse(401, "Пользователь не авторизован", typeof(string))]
        public async Task<IActionResult> Add(NewPrinterDTO model)
        {
            if (model is null) return BadRequest("Неверные данные принтера");
    
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("Пользователь не авторизован");
    
            var userId = int.Parse(userIdClaim);
            var result = await _printerService.InsertAsync(model, userId);

            if (!result.IsSuccess) return BadRequest(result.Message);
    
            return StatusCode(201, result.Message);
        }

        
        /// <summary>
        /// Удаляет принтер у пользователя.
        /// Если принтер не привязон ни к одному пользователю, то он удаляется из системы
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     DELETE /api/printer/delete/1
        /// 
        /// </remarks>
        /// <param name="id" example="1">Идентификатор принтера.</param>
        /// <returns> Возвращает сообщение о удалении принтера.</returns>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="200">Принтер успешно удален у пользователя</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        ///
        [HttpDelete("delete/{id}")]
        [SwaggerResponse(200, "Принтер успешно удален у пользователя", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        [SwaggerResponse(401, "Пользователь не авторизован", typeof(string))]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest("Неверный ID принтера");
            
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("Пользователь не авторизован");
    
            var userId = int.Parse(userIdClaim);
            var result = await _printerService.DeleteAsync(id, userId);
            if (result.HasErrors)
                return BadRequest(result.Message);
            return Ok(result.Message);
        }

        /// <summary>
        /// Возвращает список всех принтеров
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/printer/all
        ///
        /// </remarks>
        /// 
        /// <returns>
        /// Возвращает список всех принтеров
        /// </returns>
        /// <response code="200">Возвращает список всех принтеров</response>
        /// <response code="404">Принтеры не найдены</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        /// 
        [HttpGet("all")]
        [SwaggerResponse(200, "Возвращает список всех принтеров", typeof(List<Printer>))]
        [SwaggerResponse(404, "Принтеры не найдены", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        [SwaggerResponse(401, "Пользователь не авторизован", typeof(string))]
        public async Task<IActionResult> GetAll()
        {
            var result = await _printerService.GetAllAsync();
            if (result.HasErrors) return BadRequest(result.Message);
            if (!result.Data.Any()) return NotFound("Принтеры не найдены");
            return Ok(await _printerService.GetAllAsync());
        }

        /// <summary>
        /// Возвращает список принтеров по местоположению
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/printer/allByLocation/1
        ///
        ///</remarks>
        /// <param name="id" example="1">Идентификатор местоположения</param>
        /// <response code="200">Возвращает список принтеров</response>
        /// <response code="404">Принтеры не найдены</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// 
        [HttpGet("allByLocation/{id}")]
        [SwaggerResponse(200, "Возвращает список всех принтеров", typeof(List<Printer>))]
        [SwaggerResponse(404, "Принтеры не найдены", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        [SwaggerResponse(401, "Пользователь не авторизован", typeof(string))]
        public async Task<IActionResult> GetAllByLocation(int id)
        {
            if(id <= 0) return BadRequest("неверный ID местоположения");
            try
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null) return Unauthorized("Пользователь не авторизован");
    
                var userId = int.Parse(userIdClaim);
                var result = await _printerService.GetAllByLocationAsync(id, userId);
                if(result.HasErrors) return BadRequest(result.Message);
                if(result.Data.Count == 0) return NotFound("Принтеры не найдены");
                
                return Ok(result.Data);
            }
            catch
            {
                return StatusCode(401, "Пользователь не авторизован");
            }
        }

        /// <summary>
        /// Возвращает список принтеров по модели
        /// </summary>
        /// <remarks>
        ///
        ///     GET /api/printer/allByModel/1
        /// 
        /// </remarks>
        /// 
        /// <param name="id" example="1">Идентификатор модели</param>
        /// <returns>
        /// Возвращает список принтеров по модели
        /// </returns>
        ///
        /// <response code="200">Возвращает список принтеров</response>
        /// <response code="404">Принтеры не найдены</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// 
        [HttpGet("allByModel/{id}")]
        [SwaggerResponse(200, "Возвращает список всех принтеров", typeof(List<Printer>))]
        [SwaggerResponse(404, "Принтеры не найдены", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        [SwaggerResponse(401, "Пользователь не авторизован", typeof(string))]
        public async Task<IActionResult> GetAllByModel(int id)
        {
            if(id <= 0) return BadRequest("неверный ID модели");
            try
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null) return Unauthorized("Пользователь не авторизован");
    
                var userId = int.Parse(userIdClaim);
                var result = await _printerService.GetAllByModelAsync(id, userId);
                if (result.HasErrors) return BadRequest(result.Message);
                if (result.Data.Count == 0) return NotFound("Принтеры не найдены");
                return Ok(result.Data);
            }
            catch
            {
                return StatusCode(401, "Пользователь не авторизован");
            }
        }
        
        /// <summary>
        /// Возвращает список принтеров пользователя
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        /// 
        ///     GET /api/printer/allByUser
        ///
        /// </remarks>
        /// <returns>Возвращает список принтеров пользователя</returns>
        ///
        /// <response code="200">Возвращает список принтеров пользователя</response>
        /// <response code="404">Принтеры не найдены</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// 
        [HttpGet("allByUser")]
        [SwaggerResponse(200, "Возвращает список всех принтеров", typeof(List<Printer>))]
        [SwaggerResponse(404, "Принтеры не найдены", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        [SwaggerResponse(401, "Пользователь не авторизован", typeof(string))]
        public async Task<IActionResult> GetAllByUser()
        {
            try
            {
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null) return Unauthorized("Пользователь не авторизован");
    
                var userId = int.Parse(userIdClaim);
                var result = await _printerService.GetAllByUserAsync(userId);
                if (result.HasErrors) return BadRequest(result.Message);
                if (result.Data.Count == 0) return NotFound("Принтеры не найдены");
                return Ok(result.Data);
            }
            catch
            {
                return StatusCode(401, "Пользователь не авторизован");
            }
        }
        
        /// <summary>
        /// Возвращает принтер по идентификатору
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/printer/single/1
        /// 
        /// </remarks>
        /// <param name="id" example="1">Идентификатор принтера</param>
        /// <returns>Возвращает принтер по идентификатору</returns>
        /// <response code="200">Возвращает принтер</response>
        /// <response code="404">Принтер не найден</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        /// 
        [HttpGet("single/{id}")]
        [SwaggerResponse(200, "Возвращает принтер", typeof(Printer))]
        [SwaggerResponse(404, "Принтер не найдены", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0) return BadRequest("Не верный идентификатор принтера");
            var result = await _printerService.GetByIdAsync(id);
            if (result.HasErrors) return BadRequest(result.Message);
            if (!result.IsSuccess) return NotFound(result.Message);
            return Ok(result.Data);
        }

        /// <summary>
        /// Возвращает детальную информацию о принтере на основе привязанных oid
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/printer/detail/1
        /// 
        /// </remarks>
        ///
        /// <param name="id" example="1">Идентификатор принтера</param>
        /// <returns>Возвращает детальную информацию о принтере</returns>
        /// <response code="200">Возвращает принтер</response>
        /// <response code="404">Принтер не найден</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        /// 
        [HttpGet("detail/{id}")]
        [SwaggerResponse(200, "Возвращает принтер", typeof(Printer))]
        [SwaggerResponse(404, "Принтер не найдены", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        public async Task<IActionResult> GetDetail(int id)
        {
            if (id <= 0) return BadRequest("Не верный идентификатор принтера");
            var result = await _printerService.GetDetailByIdAsync(id);
            if (result.HasErrors) return BadRequest(result.Message);
            
            //TODO: Добавить обработку разных типов ошибок
            if (!result.IsSuccess) return BadRequest(result.Message);
            return Ok(result.Data);
        }
        
        /// <summary>
        /// Обновляет информацию о принтере
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     PUT /api/printer/update
        ///     {
        ///         "id": 1,
        ///         "name": "test",
        ///         "ip": "192.168.0.99",
        ///         "locationId": 1
        ///     }
        /// 
        /// </remarks>
        /// <param name="model"> Объект с информацией о принтере </param>
        /// <returns>Возвращает обновленную информацию о принтере</returns>
        /// <response code="200">Возвращает обновленную информацию о принтере</response>
        /// <response code="400">Возвращает сообщение с ошибкой</response>
        /// <response code="401">Пользователь не авторизован</response>
        /// <response code="404">Принтер не найден</response>
        /// 
        [HttpPut("update")]
        [SwaggerResponse(200, "Возвращает принтер", typeof(Printer))]
        [SwaggerResponse(404, "Принтер не найдены", typeof(string))]
        [SwaggerResponse(400, "Возвращает сообщение с ошибкой", typeof(string))]
        [SwaggerResponse(401, "Пользователь не авторизован", typeof(string))]
        public async Task<IActionResult> Update(UpdatePrinterDTO model)
        {
            if (model is null) return BadRequest("Неверные данные принтера");
            
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(r => r.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null) return Unauthorized("Пользователь не авторизован");
            var userId = int.Parse(userIdClaim);

            var result = await _printerService.UpdateAsync(model, userId);
            if (result.HasErrors) return BadRequest(result.Message);
            if(!result.IsSuccess) return NotFound(result.Message);
            return Ok(result.Data);
        }
    }
}

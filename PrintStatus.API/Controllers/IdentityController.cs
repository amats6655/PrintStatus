using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
	[Route("identity")]
	[Authorize]
	public class IdentityController : ControllerBase
	{
		[HttpGet]
		public IActionResult Get()
		{
			var claims = HttpContext.User.Claims;
			var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
			var result = "1111";
			Console.WriteLine(userId);
			return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
		}
	}

}


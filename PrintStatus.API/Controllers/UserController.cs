using Microsoft.AspNetCore.Mvc;
using PrintStatus.BLL;
using PrintStatus.DOM.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PrintStatus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase

	{
		private readonly IUserService _userService;
		public UserController(IUserService userService)
		{
			_userService = userService;
		}
		// GET: api/<UserController>
		[HttpGet]
		public IEnumerable<UserProfile> Get()
		{
			return [];
		}

		// GET api/<UserController>/5
		[HttpGet("{id}")]
		public async Task<UserProfile> Get(int id)
		{
			var result = new UserProfile() { IdentityId = Guid.NewGuid().ToString() };
			return result;
		}

		// POST api/<UserController>
		[HttpPost]
		public async Task<StatusCodeResult> Post([FromBody] AuthUserDTO value)
		{
			var result = await _userService.AddUserAsync(value.UserName, value.Password);
			if (result != null)
			{
				return Ok();
			}
			return NoContent();
		}

		// PUT api/<UserController>/5
		[HttpPut("{id}")]
		public void Put()
		{

		}

		// DELETE api/<UserController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}

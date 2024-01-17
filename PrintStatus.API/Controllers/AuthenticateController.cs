using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PrintStatus.BLL;

namespace PrintStatus.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticateController : ControllerBase
	{
		private readonly IAccountService _accountService;
		private readonly IConfiguration _configuration;
		public AuthenticateController(IAccountService accountService, IConfiguration configuration)
		{
			_accountService = accountService;
			_configuration = configuration;
		}


		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] AuthUserDTO model)
		{
			var result = await _accountService.RegisterAsync(model.UserName, model.Password);
			if (!result.Succeeded)
			{
				return BadRequest(result.Errors.Select(e => e.Description));
			}
			return Ok(new Response { Status = "Success", Message = "User created successfully" });
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] AuthUserDTO model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var result = await _accountService.LoginAsync(model.UserName, model.Password);
			if (!result.IsAuthenticated)
			{
				return Unauthorized(result.Errors);
			}
			var authClaims = new List<Claim>()
			{
				new Claim(ClaimTypes.Name, model.UserName),
				new Claim(ClaimTypes.NameIdentifier, result.IdentityUserId),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};
			foreach (var role in result.Roles)
			{
				authClaims.Add(new(ClaimTypes.Role, role));
			}
			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT: SecretKey"]));

			var token = new JwtSecurityToken(
			issuer: _configuration["JWT: ValidIssuer"],
			audience: _configuration["JWT: ValidAudience"],
			expires: DateTime.UtcNow.AddHours(4),
			claims: authClaims,
			signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
			);

			return Ok(new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token),
				expiration = token.ValidTo
			});
		}

	}
}

using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PrintStatus.WEB.Pages
{
	public class PrintersModel : PageModel
	{
		private readonly TokenManager _tokenManager;

		public PrintersModel(TokenManager tokenManager)
		{
			_tokenManager = tokenManager;
		}

		public string Json = string.Empty;

		public async Task OnGet()
		{
			var accessToken = await HttpContext.GetTokenAsync("access_token");

			if (accessToken == null)
			{
				throw new Exception("Ќе удалось получить токен доступа");
			}

			var client = new HttpClient();
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			var response = await client.GetAsync("https://localhost:6001/api/printer/user");

			if (response.StatusCode == HttpStatusCode.Unauthorized)
			{
				// ≈сли получаем 401 ответ, это может означать, что токен истек. ѕопробуем обновить токен.
				accessToken = await _tokenManager.RefreshTokenAsync();
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
				response = await client.GetAsync("https://localhost:6001/api/printer/user");
			}

			var content = await response.Content.ReadAsStringAsync();

			var parsed = JsonDocument.Parse(content);
			var formatted = JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true });

			Json = formatted;
		}
	}

}

using System.Globalization;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;

public class TokenService
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IHttpClientFactory _clientFactory;

	public TokenService(IHttpContextAccessor httpContextAccessor, IHttpClientFactory clientFactory, IConfiguration configuration)
	{
		_httpContextAccessor = httpContextAccessor;
		_clientFactory = clientFactory;
	}

	public async Task<string> GetAccessTokenAsync()
	{
		var accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");
		var expiresAt = await _httpContextAccessor.HttpContext.GetTokenAsync("expires_at");

		if (string.IsNullOrEmpty(accessToken) || TokenExpired(expiresAt))
		{
			accessToken = await RefreshAccessTokenAsync();
		}
		return accessToken;
	}

	private bool TokenExpired(string expiresAt)
	{
		return DateTime.TryParse(expiresAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var expiresAtDateTime)
			   && expiresAtDateTime < DateTime.Now;
	}

	private async Task<string> RefreshAccessTokenAsync()
	{
		var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync("refresh_token");
		var client = _clientFactory.CreateClient();

		var discoveryDocumentResponse = await client.GetDiscoveryDocumentAsync("https://localhost:5001");
		if (discoveryDocumentResponse.IsError)
		{
			throw new Exception(discoveryDocumentResponse.Error);
		}

		var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
		{
			Address = "https://localhost:5001",
			ClientId = "web",
			ClientSecret = "87D09D25-12F3-4B48-9CE5-F625E4FF7519",
			RefreshToken = refreshToken
		});

		if (tokenResponse.IsError)
		{
			throw new Exception(tokenResponse.Error);
		}

		var expiresInSeconds = tokenResponse.ExpiresIn;
		var updatedExpiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds).ToString("o", CultureInfo.InvariantCulture);

		var authInfo = await _httpContextAccessor.HttpContext.AuthenticateAsync("Cookies");
		authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
		authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);
		authInfo.Properties.UpdateTokenValue("expires_at", updatedExpiresAt);
		await _httpContextAccessor.HttpContext.SignInAsync("Cookies", authInfo.Principal, authInfo.Properties);

		return tokenResponse.AccessToken;
	}
}

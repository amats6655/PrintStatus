using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

public class TokenManager
{
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly IHttpContextAccessor _httpContextAccessor;

	public TokenManager(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
	{
		_httpClientFactory = httpClientFactory;
		_httpContextAccessor = httpContextAccessor;
	}

	public async Task<string> GetTokenAsync()
	{
		var client = _httpClientFactory.CreateClient();

		var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

		if (discoveryDocument.IsError)
		{
			throw new Exception(discoveryDocument.Error);
		}

		var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
		{
			Address = discoveryDocument.TokenEndpoint,
			ClientId = "web",
			ClientSecret = "87D09D25-12F3-4B48-9CE5-F625E4FF7519",
			Scope = "api"
		});

		if (tokenResponse.IsError)
		{
			throw new Exception(tokenResponse.Error);
		}

		return tokenResponse.AccessToken;
	}

	public async Task<string> RefreshTokenAsync()
	{
		var client = _httpClientFactory.CreateClient();

		var discoveryDocument = await client.GetDiscoveryDocumentAsync("https://localhost:5001");

		if (discoveryDocument.IsError)
		{
			throw new Exception(discoveryDocument.Error);
		}

		var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

		var tokenResponse = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
		{
			Address = discoveryDocument.TokenEndpoint,
			ClientId = "web",
			ClientSecret = "87D09D25-12F3-4B48-9CE5-F625E4FF7519",
			RefreshToken = refreshToken,
			Scope = "api"
		});

		if (tokenResponse.IsError)
		{
			throw new Exception(tokenResponse.Error);
		}

		// Обновляем токены в текущем HttpContext
		var authInfo = await _httpContextAccessor.HttpContext.AuthenticateAsync("Cookies");
		authInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.AccessToken, tokenResponse.AccessToken);
		authInfo.Properties.UpdateTokenValue(OpenIdConnectParameterNames.RefreshToken, tokenResponse.RefreshToken);
		await _httpContextAccessor.HttpContext.SignInAsync("Cookies", authInfo.Principal, authInfo.Properties);

		return tokenResponse.AccessToken;
	}
}
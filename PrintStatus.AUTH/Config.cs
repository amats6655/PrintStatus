using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace IdentityServerAspNetIdentity;

public static class Config
{
	public static IEnumerable<IdentityResource> IdentityResources =>
		new IdentityResource[]
		{
			new IdentityResources.OpenId(),
			new IdentityResources.Profile(),
		};

	public static IEnumerable<ApiScope> ApiScopes =>
		new ApiScope[]
		{
			new ApiScope("api", "PrintStatusAPI"),
		};

	public static IEnumerable<Client> Clients =>
		new Client[]
		{
			// m2m client credentials flow client
			new Client
			{
				ClientId = "client",
				ClientName = "Client Credentials Client",

				AllowedGrantTypes = GrantTypes.Code,
				ClientSecrets = { new Secret("7B8E79E7-DB39-477A-B0F5-B8B65056F140".Sha256()) },

				AllowedScopes = { "api" }
			},

			// interactive client using code flow + pkce
			new Client
			{
				ClientId = "web",
				ClientSecrets = { new Secret("87D09D25-12F3-4B48-9CE5-F625E4FF7519".Sha256()) },

				AllowedGrantTypes = GrantTypes.ClientCredentials,
			
				// where to redirect to after login
				RedirectUris = { "https://localhost:5002/signin-oidc" },

				// where to redirect to after logout
				PostLogoutRedirectUris = { "https://localhost:5002/signout-callback-oidc" },

				RefreshTokenUsage = TokenUsage.ReUse,
				AllowOfflineAccess = true,


				AllowedScopes = new List<string>
				{
					IdentityServerConstants.StandardScopes.OpenId,
					IdentityServerConstants.StandardScopes.Profile,
					"api"
				}
			},
		};
}

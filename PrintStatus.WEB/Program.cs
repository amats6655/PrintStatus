using System.IdentityModel.Tokens.Jwt;
using IdentityModel.Client;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
builder.Services.AddHttpClient();
builder.Services.AddAccessTokenManagement(options =>
{
	options.Client.Clients.Add("identityserver", new ClientCredentialsTokenRequest
	{
		Address = "https://localhost:5001/connect/token",
		ClientId = "web",
		ClientSecret = "87D09D25-12F3-4B48-9CE5-F625E4FF7519",
		Scope = "api"
	});
});

builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = "Cookies";
	options.DefaultChallengeScheme = "oidc";
})
	.AddCookie("Cookies")
	.AddOpenIdConnect("oidc", options =>
	{
		options.Authority = "https://localhost:5001";
		options.ClientId = "web";
		options.ClientSecret = "87D09D25-12F3-4B48-9CE5-F625E4FF7519";
		options.ResponseType = "code";

		options.SaveTokens = true;

		options.Scope.Clear();
		options.Scope.Add("openid");
		options.Scope.Add("profile");
		options.Scope.Add("api");
		options.Scope.Add("offline_access");
		options.GetClaimsFromUserInfoEndpoint = true;
	});
builder.Services.AddSingleton<TokenManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages().RequireAuthorization();

app.Run();

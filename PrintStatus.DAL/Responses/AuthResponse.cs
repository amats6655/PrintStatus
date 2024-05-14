namespace PrintStatus.DAL.Responses;

public class AuthResponse : IAuthResponse
{
	public bool IsSuccess { get; set; }
	public string Message { get; set; }
	public string Token { get; set; }
	public string RefreshToken { get; set; }

	public static AuthResponse Success(string message = "", string token = "", string refreshToken = "")
	{
		return new AuthResponse { IsSuccess = true, Message = message, Token = token, RefreshToken = refreshToken};
	}
	public static AuthResponse Failure(string message = "")
	{
		return new AuthResponse { IsSuccess = false, Message = message };
	}
}
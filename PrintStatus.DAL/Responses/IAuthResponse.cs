namespace PrintStatus.DAL.Responses;

public interface IAuthResponse
{
	bool IsSuccess { get; }
	string Message { get; }
	string Token { get; }
	string RefreshToken { get; }
}

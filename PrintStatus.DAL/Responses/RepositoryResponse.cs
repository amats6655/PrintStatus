namespace PrintStatus.DAL.Responses;

public class RepositoryResponse<T> : IRepositoryResponse<T>
{
	public bool IsSuccess { get; set; }
	public T Data { get; set; }
	public IEnumerable<string> Errors { get; set; } = [];
	public string Message { get; set; }

	public static RepositoryResponse<T> Success(T data, string message = null)
	{
		return new RepositoryResponse<T> { IsSuccess = true, Data = data, Message = message };
	}
	public static RepositoryResponse<T> Failure(IEnumerable<string> errors, string message = null)
	{
		return new RepositoryResponse<T> { IsSuccess = false, Errors = errors, Message = message };
	}
}

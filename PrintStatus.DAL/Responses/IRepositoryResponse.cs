namespace PrintStatus.DAL.Responses;

public interface IRepositoryResponse<T>
{
	bool IsSuccess { get; }
	T Data { get; }
	IEnumerable<string> Errors { get; }
	string Message { get; }
	
}


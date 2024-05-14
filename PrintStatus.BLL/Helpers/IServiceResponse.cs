namespace PrintStatus.BLL.Helpers;

public interface IServiceResponse<T>
{
	bool IsSuccess { get; }
	T Data { get; }
	string Message { get; }
}


namespace PrintStatus.BLL.Interfaces
{
	public interface IServiceResult<T>
	{
		bool IsSuccess { get; }
		T Data { get; }
		string Message { get; }
	}
}

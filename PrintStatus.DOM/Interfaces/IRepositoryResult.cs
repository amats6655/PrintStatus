namespace PrintStatus.DOM.Interfaces
{
	public interface IRepositoryResult<T>
	{
		bool IsSuccess { get; }
		T Data { get; }
		IEnumerable<string> Errors { get; }
		string Message { get; }
		
	}
}

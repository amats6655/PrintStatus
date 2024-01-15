using Microsoft.EntityFrameworkCore;
using PrintStatus.DOM.Interfaces;

namespace PrintStatus.DAL.Repositories
{
	public class RepositoryResult<T> : IRepositoryResult<T>
	{
		public bool IsSuccess { get; set; }
		public T Data { get; set; }
		public IEnumerable<string> Errors { get; set; } = [];
		public string Message { get; set; }

		public static RepositoryResult<T> Success(T data, string message = null)
		{
			return new RepositoryResult<T> { IsSuccess = true, Data = data, Message = message };
		}
		public static RepositoryResult<T> Failure(IEnumerable<string> errors, string message = null)
		{
			return new RepositoryResult<T> { IsSuccess = false, Errors = errors, Message = message };
		}
	}
}

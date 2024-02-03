using PrintStatus.BLL.Interfaces;

namespace PrintStatus.BLL.DTO
{
	public class ServiceResult<T> : IServiceResult<T>
	{
		public bool IsSuccess { get; set; }
		public T Data { get; set; }
		public string Message { get; set; }

		public static ServiceResult<T> Success(T data, string message = null)
		{
			return new ServiceResult<T> { IsSuccess = true, Data = data, Message = message };
		}
		public static ServiceResult<T> Failure(string message)
		{
			return new ServiceResult<T> { IsSuccess = false, Message = message };
		}
	}


}


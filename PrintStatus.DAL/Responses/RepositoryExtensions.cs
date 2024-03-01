namespace PrintStatus.DAL.Responses;

using Microsoft.EntityFrameworkCore;

public static class RepositoryExtensions
{
	public static IRepositoryResponse<T> HandleException<T>(this IRepositoryResponse<T> repositoryResponse, Exception ex)
	{
		var errors = new List<string> { ex.Message };
		var message = "";
		switch (ex)
		{
			case ArgumentNullException:
				message = "Один из аргументов был null или пустым значением";
				break;
			// case ArgumentException:
			// 	errors.Add("Объект уже существует в базе данных");
			// 	break;
			case DbUpdateConcurrencyException:
				message = "Кто-то уже изменил данный объект";
				break;
			case OperationCanceledException:
				message = "Задача была отклонена базой данных";
				break;
			case DbUpdateException:
				message = "Ошибка обновления базы данных";
				break;
			default:
				message = "Неизвестная ошибка";
				break;
		}

		return RepositoryResponse<T>.Failure(errors, message);
	}
}
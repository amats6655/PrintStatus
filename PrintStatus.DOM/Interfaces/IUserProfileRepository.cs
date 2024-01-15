using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface IUserProfileRepository
	{

		Task<IRepositoryResult<UserProfile>> GetUserByIdAsync(int id);
		/// <summary>
		/// Данный метод создает UserProfile
		/// </summary>
		/// <param name="identityUserId">
		/// Guid пользователя IdentityUser
		/// </param>
		/// <returns> 
		/// Если RepositoryResult.Data == null, то посмотреть ошибки в RepositoryResult.Errors
		/// <para>Если RepositoryResult.Data != null, то в Data лежит созданный UserProfile</para>
		/// 
		/// </returns>
		Task<IRepositoryResult<UserProfile>> AddUserProfileAsync(string identityUserId);
		Task<IRepositoryResult<UserProfile>> GetUserByIdentityId(string IdentityUserId);
	}
}

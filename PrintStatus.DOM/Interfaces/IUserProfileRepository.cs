using PrintStatus.DOM.Models;

namespace PrintStatus.DOM.Interfaces
{
	public interface IUserProfileRepository
	{
		Task<UserProfile> GetUserByIdAsync(int id);
		Task<UserProfile> AddUserProfileAsync(string identityUserId);
		Task<UserProfile> GetUserByIdentityId(string IdentityUserId);
	}
}

using Microsoft.AspNetCore.Identity;

namespace PrintStatus.BLL;

public interface IUserService
{
	Task<IdentityResult> AddUserAsync(string userName, string password);
	Task<IdentityResult> UpdateUserAsync(string userId, string newUserName);
	Task<IdentityResult> AssignRoleAsync(string userName, string roleName);
}

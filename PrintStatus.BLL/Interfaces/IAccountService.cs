using Microsoft.AspNetCore.Identity;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;

namespace PrintStatus.BLL;

public interface IAccountService
{
	Task<AuthResult> LoginAsync(string username, string password);
	Task<IdentityResult> RegisterAsync(string username, string password);
	Task<IdentityResult> AssignRoleAsync(string userName, string roleName);
	Task<IdentityResult> UpdateUserAsync(string userId, string newUserName);
	Task<IdentityResult> DeleteUserAsync(string userId);
	Task<IdentityResult> UpdateRoleAsync(string userId, string roleName);
	Task<IdentityResult> UpdatePassword(string userId, string oldPassword, string newPassword);
	Task<IServiceResult<IEnumerable<string>>> GetRolesAsync(string userId);
}

namespace PrintStatus.DAL.Repositories.Interfaces;

using DOM.Models;
using DTOs;
using Responses;

public interface IUserAccount
{
	Task<IRepositoryResponse<Register>> CreateAsync(Register user);
	Task<IAuthResponse> SignInAsync(Login user);
	Task<IRepositoryResponse<ApplicationUser>> GetUserById(int id);
	Task<IAuthResponse> RefreshTokenAsync(RefreshToken refreshToken);
	Task<IRepositoryResponse<List<ManageUser>>> GetUsers();
	Task<IRepositoryResponse<ManageUser>> UpdateUser(ManageUser user);
	Task <List<SystemRole>> GetRoles();
	Task<IRepositoryResponse<bool>> DeleteUser(int id);
}

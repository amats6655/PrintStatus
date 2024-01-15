using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
	public class UserProfileRepository : IUserProfileRepository
	{
		private readonly ApplicationDbContext _context;
		public UserProfileRepository(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IRepositoryResult<UserProfile>> AddUserProfileAsync(string identityUserId)
		{
			if(string.IsNullOrWhiteSpace(identityUserId)) return new RepositoryResult<UserProfile>().HandleException(new ArgumentNullException(nameof(identityUserId)));
			var userExist = await _context.UserProfiles.AnyAsync(u => u.IdentityId.Equals(identityUserId));
			if (userExist) return RepositoryResult<UserProfile>.Failure(new List<string> {""}, "Пользователь с таким identityUserId уже существует");
			try
			{
				var userProfile = new UserProfile() { IdentityId = identityUserId, Printer = new List<BasePrinter>() };
				await _context.UserProfiles.AddAsync(userProfile);
				await _context.SaveChangesAsync();
				return RepositoryResult<UserProfile>.Success(userProfile, "Пользователь успешно добавлен");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<UserProfile>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<UserProfile>> GetUserByIdAsync(int id)
		{
			if(id <= 0) return new RepositoryResult<UserProfile>().HandleException(new ArgumentNullException(nameof(id)));
			try
			{
				var result = await _context.UserProfiles.FindAsync(id);
				if (result == null) return RepositoryResult<UserProfile>.Failure(new List<string> {""}, "Пользователь не найден");
				return RepositoryResult<UserProfile>.Success(result, "Пользователь найден"); ;
			}
			catch (Exception ex)
			{
				return new RepositoryResult<UserProfile>().HandleException(ex);
			}
		}

		public async Task<IRepositoryResult<UserProfile>> GetUserByIdentityId(string identityUserId)
		{
			if(string.IsNullOrWhiteSpace(identityUserId)) return new RepositoryResult<UserProfile>().HandleException(new ArgumentNullException(nameof(identityUserId)));
			try
			{
				var result = await _context.UserProfiles
									.Where(u => u.IdentityId.Equals(identityUserId))
									.FirstOrDefaultAsync();
				if (result == null) return RepositoryResult<UserProfile>.Failure(new List<string> {""}, "Пользователь не найден");
				return RepositoryResult<UserProfile>.Success(result, "Пользователь найден");
			}
			catch (Exception ex)
			{
				return new RepositoryResult<UserProfile>().HandleException(ex);			
			}
		}
	}
}
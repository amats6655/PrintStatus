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

		public async Task<UserProfile> AddUserProfileAsync(string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(identityUserId, nameof(identityUserId));
			try
			{
				var userProfile = new UserProfile() { IdentityId = identityUserId, Printer = new List<BasePrinter>()};
				await _context.UserProfiles.AddAsync(userProfile);
				await _context.SaveChangesAsync();
				return userProfile;
			}
			catch(Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<UserProfile> GetUserByIdAsync(int id)
		{
			try
			{
				return await _context.UserProfiles.FindAsync(id);
			}
			catch (Exception ex)
			{
				//TODO Добавить обработчик ошибок
				Console.WriteLine(ex.Message);
				return null;
			}
		}

		public async Task<UserProfile> GetUserByIdentityId(string identityUserId)
		{
			ArgumentException.ThrowIfNullOrEmpty(identityUserId, nameof(identityUserId));
			var result = await _context.UserProfiles
									.Where(u => u.IdentityId.Equals(identityUserId))
									.FirstOrDefaultAsync();
			if (result != null)
			{
				return result;
			}
			//TODO добавить обработку null
			return null;
		}
	}
}
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
    }
}
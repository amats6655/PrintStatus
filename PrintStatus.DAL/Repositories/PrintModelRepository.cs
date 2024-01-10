using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
    public class PrintModelRepository : IPrintModelRepository
    {
        private readonly ApplicationDbContext _context;

        public PrintModelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PrintModel> AddAsync(PrintModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            try
            {
                _context.PrintModels.Add(model);
                await _context.SaveChangesAsync();
                return model;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(PrintModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            try
            {
                _context.PrintModels.Remove(model);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<IEnumerable<PrintModel>> GetAllAsync()
        {
            try
            {
                return await _context.PrintModels.ToListAsync();

            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<PrintModel>();
            }
        }

        public async Task<PrintModel> GetByIdAsync(int id)
        {
            try
            {
                return await _context.PrintModels.FindAsync(id);

            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<int> GetIdByModelNameAsync(string modelName)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(modelName);
            try
            {
                return await _context.PrintModels
                    .Where(m => m.Title == modelName)
                    .Select(m => m.Id)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                //TODO Добавить обрабочик ошибок
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public async Task<PrintModel> UpdateAsync(PrintModel model)
        {
            ArgumentNullException.ThrowIfNull(model);
            try
            {
                _context.PrintModels.Update(model);
                await _context.SaveChangesAsync();
                return model;
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

using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
    public class OidRepository : IOidRepository
    {
        private readonly ApplicationDbContext _context;

        public OidRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Oid> AddAsync(Oid oid)
        {
            ArgumentNullException.ThrowIfNull(oid);
            try
            {
                _context.Oids.Add(oid);
                await _context.SaveChangesAsync();
                return oid;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteAsync(Oid oid)
        {
            ArgumentNullException.ThrowIfNull(oid);
            try
            {
                _context.Oids.Remove(oid);
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

        public async Task<IEnumerable<Oid>> GetAllAsync()
        {
            try
            {
                var oids = await _context.Oids.ToListAsync();
                return oids;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<Oid>();
            }
        }

        public async Task<Oid> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Oids.FindAsync(id);
            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<Oid> UpdateAsync(Oid oid)
        {
            ArgumentNullException.ThrowIfNull(oid);
            try
            {
                _context.Oids.Update(oid);
                await _context.SaveChangesAsync();
                return oid;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<IEnumerable<Oid>> GetAllByModelIdAsync(int modelId)
        {
            try
            {
                return await _context.PrintModels
                    .Where(m => m.Id == modelId)
                    .SelectMany(o => o.Oids)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return Enumerable.Empty<Oid>();
            }
        }
    }
}

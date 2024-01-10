using Microsoft.EntityFrameworkCore;
using PrintStatus.DAL.Connection;
using PrintStatus.DOM.Interfaces;
using PrintStatus.DOM.Models;

namespace PrintStatus.DAL.Repositories
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;

        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AuditLog> AddAsync(AuditLog auditLog)
        {
            ArgumentNullException.ThrowIfNull(auditLog, nameof(auditLog));
            try
            {
                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
                return auditLog;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработку ошибок
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            var result = new List<AuditLog>();
            try
            {
                result = await _context.AuditLogs.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработку ошибок
                Console.WriteLine(ex.Message);
                return result;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByDateAsync(DateTime date)
        {
            var result = new List<AuditLog>();
            try
            {
                result = await _context.AuditLogs
                                    .Where(h => h.Date == date)
                                    .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработчик ошибок
                Console.WriteLine(ex.Message);
                return result;
            }
        }

        public async Task<AuditLog> GetByIdAsync(int id)
        {
            AuditLog result = null;
            try
            {
                result = await _context.AuditLogs.FindAsync(id);
                return result;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработку ошибок
                Console.WriteLine(ex.Message);
                return result;
            }
        }

        //public async Task<IEnumerable<AuditLog>> GetByPrinterAsync(int printerId)
        //{
        //    var result = new List<AuditLog>();
        //    try
        //    {
        //        result = await _context.AuditLogs
        //                            .Where(a => a.BasePrinterId == printerId)
        //                            .ToListAsync();
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        //TODO Добавить обработку ошибок
        //        Console.WriteLine(ex.Message);
        //        return result;
        //    }
        //}

        public async Task<IEnumerable<AuditLog>> GetByUserAsync(int userId)
        {
            var result = new List<AuditLog>();
            try
            {
                result = await _context.AuditLogs
                                    .Where(a => a.UserId == userId)
                                    .ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                //TODO Добавить обработку ошибок
                Console.WriteLine(ex.Message);
                return result;
            }
        }
    }
}

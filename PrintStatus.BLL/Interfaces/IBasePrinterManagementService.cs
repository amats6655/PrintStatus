using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Interfaces
{
    public interface IBasePrinterManagementService
    {
        Task<PrinterDTO> GetByIdAsync(int id);
        Task<PrinterDetailDTO> GetDetailByIdAsync(int id);
        Task<PrinterDTO> AddAsync(string title, string ipAddress, int locationId, int userProfileId);
        Task<PrinterDTO> UpdateAsync(PrinterDTO printer, int userProfileId);
        Task<bool> DeleteAsync(int id, int userProfileId);

    }
}

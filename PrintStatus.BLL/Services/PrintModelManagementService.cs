using PrintStatus.BLL.Interfaces;

namespace PrintStatus.BLL.Services
{
    public class PrintModelManagementService : IPrintModelManagementService
    {
        public Task<PrintModelDTO> Add(PrintModelDTO printModel)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PrintModelDTO>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<PrintModelDTO> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PrintModelDTO> Update(PrintModelDTO printModel)
        {
            throw new NotImplementedException();
        }
    }
}

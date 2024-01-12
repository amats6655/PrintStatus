using System.Security.Cryptography;
using PrintStatus.BLL.DTO;
using PrintStatus.BLL.Interfaces;

namespace PrintStatus.BLL.Services
{
    public class OidManagementService : IOidManagementService
    {
        public Task<Oid> Add(OidDTO oid)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(int oidId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Oid>> GetAllByModel(int modelId)
        {
            throw new NotImplementedException();
        }

        public Task<Oid> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Oid> Update(OidDTO oid)
        {
            throw new NotImplementedException();
        }
    }
}

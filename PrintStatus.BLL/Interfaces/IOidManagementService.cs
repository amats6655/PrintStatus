using System.Security.Cryptography;
using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Interfaces
{
	public interface IOidManagementService
	{
		Task<Oid> Add(OidDTO oid);
		Task<Oid> GetById(int id);
		Task<IEnumerable<Oid>> GetAllByModel(int modelId);
		Task<Oid> Update (OidDTO oid);
		Task<bool> Delete (int oidId);
	}
}

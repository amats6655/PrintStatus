using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Interfaces
{
	public interface IPrintOidManagementService
	{
		Task<OidDTO> AddAsync(OidDTO oid);
		Task<OidDTO> GetByIdAsync(int id);
		Task<IEnumerable<OidDTO>> GetAllByModelAsync(int modelId);
		Task<OidDTO> UpdateAsync(OidDTO oid);
		Task<bool> DeleteAsync(int oidId);
	}
}

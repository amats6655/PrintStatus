using PrintStatus.BLL.DTO;

namespace PrintStatus.BLL.Interfaces
{
	public interface IPrintOidManagementService
	{
		Task<IServiceResult<OidDTO>> AddAsync(OidDTO oid);
		Task<IServiceResult<OidDTO>> GetByIdAsync(int id);
		Task<IServiceResult<IEnumerable<OidDTO>>> GetAllByModelAsync(int modelId);
		Task<IServiceResult<OidDTO>> UpdateAsync(OidDTO oid);
		Task<IServiceResult<bool>> DeleteAsync(int oidId);
	}
}

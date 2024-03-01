namespace PrintStatus.BLL.Services.Interfaces;

using DTO;
using Helpers;

public interface IPrintOidManagementService
{
	Task<IServiceResponse<OidDTO>> AddAsync(OidDTO oid, int printModelId);
	Task<IServiceResponse<OidDTO>> GetByIdAsync(int id);
	Task<IServiceResponse<IEnumerable<OidDTO>>> GetAllByModelAsync(int modelId);
	Task<IServiceResponse<OidDTO>> UpdateAsync(OidDTO oid, string userIdentityId);
	Task<IServiceResponse<bool>> DeleteAsync(int oidId);
}


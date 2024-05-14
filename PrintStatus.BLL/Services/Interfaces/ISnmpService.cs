namespace PrintStatus.BLL.Services.Interfaces;

using Helpers;
using Lextm.SharpSnmpLib;

public interface ISnmpService
{
	Task<IServiceResponse<Dictionary<string, string>>> GetModelAndSerialNumAsync(string ipAddress);
	Task<IServiceResponse<IEnumerable<Variable>>> GetOidsAsync(string ipAddress, List<Variable> oids);
}


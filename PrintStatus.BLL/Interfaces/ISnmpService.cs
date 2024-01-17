using Lextm.SharpSnmpLib;

namespace PrintStatus.BLL.Interfaces
{
	public interface ISnmpService
	{
		Task<IServiceResult<Dictionary<string, string>>> GetModelAndSerialNumAsync(string ipAddress);
		Task<IServiceResult<IEnumerable<Variable>>> GetOidsAsync(string ipAddress, List<Variable> oids);
	}
}

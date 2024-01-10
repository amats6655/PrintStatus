using Lextm.SharpSnmpLib;

namespace PrintStatus.BLL.Interfaces
{
    public interface ISnmpService
    {
        Task<Dictionary<string, string>> GetModelAndSerialNumAsync(string ipAddress);
        Task<List<Variable>> GetOidsAsync(string ipAddress, List<Variable> oids);
    }
}

using System.Net;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;
using PrintStatus.BLL.Interfaces;

namespace PrintStatus.BLL.Services
{
    public class SnmpService : ISnmpService
    {
        const int PORT = 161;
        internal Variable SerialNumber = new(new ObjectIdentifier("1.3.6.1.2.1.43.5.1.1.17.1"));
        internal Variable Model = new(new ObjectIdentifier("1.3.6.1.2.1.25.3.2.1.3.1"));
        public SnmpService()
        {

        }
        public async Task<Dictionary<string, string>> GetModelAndSerialNumAsync(string ipAddress)
        {
            var Result = new Dictionary<string, string>();
            var baseoids = new List<Variable>() { Model, SerialNumber };
            try
            {
                var value = await Messenger.GetAsync(VersionCode.V2,
                                new IPEndPoint(IPAddress.Parse(ipAddress), PORT),
                                new OctetString("public"),
                                baseoids);
                Result.Add("Model", value[0].Data.ToString());
                Result.Add("SerialNumber", value[1].Data.ToString());
            }
            catch (Exception ex)
            {
                //TODO Написать обработчик ошибок
                Console.WriteLine(ex.Message);
            }
            return Result;
        }

        public async Task<List<Variable>> GetOidsAsync(string ipAddress, List<Variable> oids)
        {
            try
            {
                return await Messenger.GetAsync(VersionCode.V2,
                            new IPEndPoint(IPAddress.Parse(ipAddress), PORT),
                            new OctetString("public"),
                            oids) as List<Variable>;
            }
            catch (Exception ex)
            {
                //TODO Написать обработчик ошибок
                Console.WriteLine(ex.Message);
                return new List<Variable>();
            }
        }
    }
}

namespace PrintStatus.BLL.Services.Implementations;

using System.Net;
using System.Net.NetworkInformation;
using DTO;
using Helpers;
using Interfaces;
using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Messaging;

public class SnmpService : ISnmpService
{
	private const int PORT = 161;
	private readonly Variable _serialNumber = new(new ObjectIdentifier("1.3.6.1.2.1.43.5.1.1.17.1"));
	private readonly Variable _model = new(new ObjectIdentifier("1.3.6.1.2.1.25.3.2.1.3.1"));


	public async Task<IServiceResponse<Dictionary<string, string>>> GetModelAndSerialNumAsync(string ipAddress)
	{
		if (string.IsNullOrEmpty(ipAddress)) return ServiceResponse<Dictionary<string, string>>.Failure("Неверный ip адрес");
		IPAddress address;
		try
		{
			address = IPAddress.Parse(ipAddress);
		}
		catch
		{
			return ServiceResponse<Dictionary<string, string>>.Failure("Неверный формат адреса");
		}
		using var ping = new Ping();
		PingReply pingRes = await ping.SendPingAsync(ipAddress, 100);
		if (pingRes.Status != IPStatus.Success) return ServiceResponse<Dictionary<string, string>>.Failure("Принтер недоступен");
		var result = new Dictionary<string, string>();
		var baseOids = new List<Variable>() { _model, _serialNumber };
		try
		{
			var value = await Messenger.GetAsync(VersionCode.V2,
						new IPEndPoint(address, PORT),
						new OctetString("public"),
						baseOids);
			result.Add("Model", value[0].Data.ToString());
			result.Add("SerialNumber", value[1].Data.ToString());
		}
		catch (Exception ex)
		{
			return ServiceResponse<Dictionary<string, string>>.Failure($"{ex.Message}");
		}
		if (result.Count != 2) return ServiceResponse<Dictionary<string, string>>.Failure("Не удалось получить информацию от принтера");
		return ServiceResponse<Dictionary<string, string>>.Success(result);

	}

	public async Task<IServiceResponse<IEnumerable<Variable>>> GetOidsAsync(string ipAddress, List<Variable> oids)
	{
		if (string.IsNullOrEmpty(ipAddress)) return ServiceResponse<IEnumerable<Variable>>.Error("Неверный ip адрес");
		if (oids.Count == 0) return ServiceResponse<IEnumerable<Variable>>.Failure("Пустой список Oid");
		IPAddress address;
		try
		{
			address = IPAddress.Parse(ipAddress);
		}
		catch
		{
			return ServiceResponse<IEnumerable<Variable>>.Error("Неверный формат адреса");
		}
		using var ping = new Ping();
		PingReply pingRes = await ping.SendPingAsync(ipAddress, 100);
		if (pingRes.Status != IPStatus.Success) return ServiceResponse<IEnumerable<Variable>>.Failure("Принтер недоступен");
		var result = new List<Variable>();
		try
		{
			var values = await Messenger.GetAsync(VersionCode.V2,
						new IPEndPoint(address, PORT),
						new OctetString("public"),
						oids);
			foreach (var t in values)
			{
				result.Add(t);
			}
		}
		catch (Exception ex)
		{
			return ServiceResponse<IEnumerable<Variable>>.Error(ex.Message);
		}
		if (result.Count == 0) return ServiceResponse<IEnumerable<Variable>>.Failure("Не удалось получить информацию от принтера");
		return ServiceResponse<IEnumerable<Variable>>.Success(result);
	}
}

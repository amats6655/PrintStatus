namespace PrintStatus.BLL.Services.Implementations;

using Interfaces;

public class PollingStateService : IPollingStateService
{
	public Dictionary<int, Dictionary<int, DateTime>> LastPolledTimes { get; } = new Dictionary<int, Dictionary<int, DateTime>>();
}


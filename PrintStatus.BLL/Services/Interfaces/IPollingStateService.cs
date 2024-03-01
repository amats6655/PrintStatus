namespace PrintStatus.BLL.Services.Interfaces;

public interface IPollingStateService
{
	Dictionary<int, Dictionary<int, DateTime>> LastPolledTimes { get; }
}


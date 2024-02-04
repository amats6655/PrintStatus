namespace PrintStatus.BLL.Interfaces
{
	public interface IPollingStateService
	{
		Dictionary<int, Dictionary<int, DateTime>> LastPolledTimes { get; }
	}
}

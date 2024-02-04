using PrintStatus.BLL.Interfaces;

namespace PrintStatus.BLL.Services
{
	public class PollingStateService : IPollingStateService
	{
		public Dictionary<int, Dictionary<int, DateTime>> LastPolledTimes { get; } = new Dictionary<int, Dictionary<int, DateTime>>();
	}
}

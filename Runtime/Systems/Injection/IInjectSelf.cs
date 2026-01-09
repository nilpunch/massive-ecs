namespace Massive
{
	public interface IInjectSelf<TSelf> : ISystemsCallbacks where TSelf : IInjectSelf<TSelf>
	{
		void ISystemsCallbacks.AfterBuilded(Systems systems) => systems.Inject((TSelf)this);
	}
}

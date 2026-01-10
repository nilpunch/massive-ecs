namespace Massive
{
	public interface IInjectSelf<TSelf> : ISystemsCallback where TSelf : IInjectSelf<TSelf>
	{
		void ISystemsCallback.AfterBuilded(Systems systems) => systems.Inject((TSelf)this);
	}
}

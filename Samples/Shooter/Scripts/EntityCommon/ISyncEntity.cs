namespace Massive.Samples.Shooter
{
	public interface ISyncEntity<TState> : ISyncComponent<TState>
	{
		void Enable();
		void Disable();
	}
}

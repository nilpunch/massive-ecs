namespace Massive.Samples.Shooter
{
	public interface ISyncEntity<TState> : ISyncComponent<TState> where TState : unmanaged
	{
		void Enable();
		void Disable();
	}
}

namespace Massive.Samples.Shooter
{
	public interface ISyncComponent<TState>
	{
		void SyncState(ref TState state);
	}
}

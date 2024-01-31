namespace Massive.Samples.Shooter
{
	public interface ISyncComponent<TState>
	{
		void SyncState(WorldFrame worldFrame, ref TState state);
	}
}
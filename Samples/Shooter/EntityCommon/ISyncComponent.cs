namespace Massive.Samples.Shooter
{
	public interface ISyncComponent<TState>
	{
		void SyncState(World world, ref TState state);
	}
}
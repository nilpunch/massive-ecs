namespace MassiveData.Samples.Shooter
{
	public interface ISyncComponent<TState>
	{
		void SyncState(ref TState state);
	}
}
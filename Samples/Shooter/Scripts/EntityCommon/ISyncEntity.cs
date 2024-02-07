namespace MassiveData.Samples.Shooter
{
	public interface ISyncEntity<TState> : ISyncComponent<TState> where TState : struct
	{
		void Enable();
		void Disable();
	}
}
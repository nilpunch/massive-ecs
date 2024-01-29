namespace Massive.Samples.Shooter
{
	public interface IWorldEntity<TState> : IWorldComponent<TState> where TState : struct
	{
		void Enable();
		void Disable();
	}
}
namespace Massive.Samples.Shooter
{
	public interface IUpdateComponent<TState>
	{
		void UpdateState(World world, ref TState state);
	}
}
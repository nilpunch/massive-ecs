namespace Massive.Samples.ECS
{
	public interface IView<T1, T2>
	{
		void ForEach(EntityActionRef<T1, T2> action);
	}
}
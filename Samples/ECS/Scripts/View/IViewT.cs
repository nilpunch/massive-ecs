namespace Massive.Samples.ECS
{
	public interface IView<T>
	{
		void ForEach(EntityActionRef<T> action);
	}
}
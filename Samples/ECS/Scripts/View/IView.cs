namespace Massive.Samples.ECS
{
	public interface IView
	{
		void ForEach(EntityAction action);
	}
}
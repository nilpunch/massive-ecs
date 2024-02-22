namespace Massive.Samples.ECS
{
	public interface IComponentMassive : IMassive, IReadOnlySet
	{
		void DeleteById(int id);
	}
}
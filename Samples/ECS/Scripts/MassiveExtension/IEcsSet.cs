namespace Massive.Samples.ECS
{
	public interface IEcsSet : IMassive, IReadOnlySet
	{
		void DeleteById(int id);
	}
}
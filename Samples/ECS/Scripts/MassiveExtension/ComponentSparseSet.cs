namespace Massive.Samples.ECS
{
	public class ComponentSparseSet : MassiveSparseSet, IComponentMassive
	{
		public ComponentSparseSet(int framesCapacity = 121, int dataCapacity = 100)
			: base(framesCapacity, dataCapacity)
		{
		}
		
		public void DeleteById(int id)
		{
			Delete(id);
		}
	}
}
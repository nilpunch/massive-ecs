namespace Massive.Samples.ECS
{
	public class ComponentDataSet<T> : MassiveDataSet<T>, IComponentMassive where T : struct
	{
		public ComponentDataSet(int framesCapacity = 121, int dataCapacity = 100)
			: base(framesCapacity, dataCapacity)
		{
		}
		
		public void DeleteById(int id)
		{
			Delete(id);
		}
	}
}
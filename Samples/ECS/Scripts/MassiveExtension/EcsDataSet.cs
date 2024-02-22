namespace Massive.Samples.ECS
{
	public class EcsDataSet<T> : MassiveDataSet<T>, IEcsSet where T : struct
	{
		public EcsDataSet(int framesCapacity = 121, int dataCapacity = 100)
			: base(framesCapacity, dataCapacity)
		{
		}

		public void DeleteById(int id)
		{
			Delete(id);
		}
	}
}
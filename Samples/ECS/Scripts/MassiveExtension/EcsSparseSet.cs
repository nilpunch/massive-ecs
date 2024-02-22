namespace Massive.Samples.ECS
{
	public class EcsSparseSet : MassiveSparseSet, IEcsSet
	{
		public EcsSparseSet(int framesCapacity = 121, int dataCapacity = 100)
			: base(framesCapacity, dataCapacity)
		{
		}

		public void DeleteById(int id)
		{
			Delete(id);
		}
	}
}
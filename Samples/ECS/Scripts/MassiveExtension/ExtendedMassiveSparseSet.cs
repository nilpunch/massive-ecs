using MassiveData;

namespace Massive.Samples.ECS
{
	public class ExtendedMassiveSparseSet : MassiveSparseSet, IExtendedMassive
	{
		public ExtendedMassiveSparseSet(int framesCapacity = 121, int dataCapacity = 100)
			: base(framesCapacity, dataCapacity)
		{
		}
		
		public void DeleteById(int id)
		{
			Delete(id);
		}
	}
}
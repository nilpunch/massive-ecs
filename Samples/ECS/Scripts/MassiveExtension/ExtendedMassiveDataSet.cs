using Massive;

namespace Massive.Samples.ECS
{
	public class ExtendedMassiveDataSet<T> : MassiveDataSet<T>, IExtendedMassive where T : struct
	{
		public ExtendedMassiveDataSet(int framesCapacity = 121, int dataCapacity = 100)
			: base(framesCapacity, dataCapacity)
		{
		}
		
		public void DeleteById(int id)
		{
			Delete(id);
		}
	}
}
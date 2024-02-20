using MassiveData;

namespace Massive.Samples.ECS
{
	public class ExtendedMassive<T> : Massive<T>, IExtendedMassive where T : struct
	{
		public ExtendedMassive(int framesCapacity = 121, int dataCapacity = 100)
			: base(framesCapacity, dataCapacity)
		{
		}
		
		public void DeleteById(int id)
		{
			Delete(id);
		}
	}
}
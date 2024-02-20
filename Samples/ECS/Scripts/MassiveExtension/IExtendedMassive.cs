using MassiveData;

namespace Massive.Samples.ECS
{
	public interface IExtendedMassive : IMassive
	{
		void DeleteById(int id);
	}
}
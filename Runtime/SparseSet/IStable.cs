using System.Linq;

namespace Massive
{
	public interface IStable
	{
		static bool IsImplementedFor<T>()
		{
			return typeof(T).GetInterfaces().Any(@interface => @interface == typeof(IStable));
		}
	}
}

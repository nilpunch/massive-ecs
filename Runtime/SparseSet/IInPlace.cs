using System.Linq;

namespace Massive
{
	public interface IInPlace
	{
		static bool IsImplementedFor<T>()
		{
			return typeof(T).GetInterfaces()
				.Any(@interface => @interface == typeof(IInPlace));
		}
	}
}

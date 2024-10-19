using System.Linq;

namespace Massive
{
	/// <summary>
	/// Marker for components that use <see cref="PackingMode"/>.<see cref="PackingMode.WithHoles"/>.
	/// </summary>
	public interface IStable
	{
		static bool IsImplementedFor<T>()
		{
			return typeof(T).GetInterfaces().Any(@interface => @interface == typeof(IStable));
		}
	}
}

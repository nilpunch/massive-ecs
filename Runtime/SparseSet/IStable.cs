using System;
using System.Linq;

namespace Massive
{
	/// <summary>
	/// Marker for components that use <see cref="PackingMode"/>.<see cref="PackingMode.WithHoles"/>.
	/// </summary>
	public interface IStable
	{
		public static bool IsImplementedFor(Type type)
		{
			return type.GetInterfaces().Any(@interface => @interface == typeof(IStable));
		}
	}
}

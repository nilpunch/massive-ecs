using System;
using System.Linq;

namespace Massive
{
	public static class ManagedUtils
	{
		public static bool IsManaged<T>()
		{
			return typeof(T).GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IManaged<>));
		}

		/// <summary>
		/// Create <see cref="Massive.MassiveManagedDataSet{T}"/> bypassing <see cref="Massive.IManaged{T}"/> constraint.
		/// </summary>
		public static IDataSet<T> CreateMassiveManagedDataSet<T>(int setCapacity = Constants.DefaultSetCapacity,
			int framesCapacity = Constants.DefaultFramesCapacity, int pageSize = Constants.DefaultPageSize)
		{
			var constructedType = typeof(MassiveManagedDataSet<>).MakeGenericType(typeof(T));

			return (IDataSet<T>)Activator.CreateInstance(constructedType, setCapacity, framesCapacity, pageSize);
		}
	}
}

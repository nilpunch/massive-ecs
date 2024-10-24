using System;
using System.Linq;

namespace Massive
{
	public static class ManagedUtils
	{
		public static bool IsManaged<T>()
		{
			return IsManaged(typeof(T));
		}

		public static bool IsManaged(Type type)
		{
			return type.GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IManaged<>));
		}

		/// <summary>
		/// Create <see cref="Massive.MassiveManagedDataSet{T}"/> bypassing <see cref="Massive.IManaged{T}"/> constraint.
		/// </summary>
		public static DataSet<T> CreateMassiveManagedDataSet<T>(int setCapacity = Constants.DefaultCapacity,
			int framesCapacity = Constants.DefaultFramesCapacity, int pageSize = Constants.DefaultPageSize, PackingMode packingMode = PackingMode.Continuous)
		{
			return (DataSet<T>)ReflectionHelpers.CreateGeneric(typeof(MassiveManagedDataSet<>), typeof(T), setCapacity, framesCapacity, pageSize, packingMode);
		}
	}
}

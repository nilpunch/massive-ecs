using System;
using System.Linq;

namespace Massive
{
	public static class ManagedUtils
	{
		public static bool IsManaged(Type type)
		{
			return type.GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IManaged<>));
		}

		/// <summary>
		/// Create <see cref="Massive.MassiveManagedDataSet{T}"/> bypassing <see cref="Massive.IManaged{T}"/> constraint.
		/// </summary>
		public static DataSet<T> CreateMassiveManagedDataSet<T>(int framesCapacity = Constants.DefaultFramesCapacity, int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
		{
			return (DataSet<T>)ReflectionUtils.CreateGeneric(typeof(MassiveManagedDataSet<>), typeof(T), framesCapacity, pageSize, packing);
		}
	}
}

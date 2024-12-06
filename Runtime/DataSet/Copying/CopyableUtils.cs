using System;
using System.Linq;

namespace Massive
{
	public static class CopyableUtils
	{
		public static bool IsImplementedFor(Type type)
		{
			return type.GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICopyable<>));
		}

		/// <summary>
		/// Create <see cref="MassiveCopyingDataSet{T}"/> bypassing <see cref="ICopyable{T}"/> constraint.
		/// </summary>
		public static DataSet<T> CreateMassiveCopyableDataSet<T>(int framesCapacity = Constants.DefaultFramesCapacity, int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
		{
			return (DataSet<T>)ReflectionUtils.CreateGeneric(typeof(MassiveCopyingDataSet<>), typeof(T), framesCapacity, pageSize, packing);
		}

		/// <summary>
		/// Create <see cref="CopyingDataSet{T}"/> bypassing <see cref="ICopyable{T}"/> constraint.
		/// </summary>
		public static DataSet<T> CreateCopyableDataSet<T>(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
		{
			return (DataSet<T>)ReflectionUtils.CreateGeneric(typeof(CopyingDataSet<>), typeof(T), pageSize, packing);
		}
	}
}

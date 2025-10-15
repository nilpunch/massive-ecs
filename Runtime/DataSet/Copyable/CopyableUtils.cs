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
		/// Create <see cref="CopyingDataSet{T}"/> bypassing <see cref="ICopyable{T}"/> constraint.
		/// </summary>
		public static DataSet<T> CreateCopyingDataSet<T>(T defaultValue = default)
		{
			return (DataSet<T>)ReflectionUtils.CreateGeneric(typeof(CopyingDataSet<>), typeof(T), defaultValue);
		}

		/// <summary>
		/// Create <see cref="CopyingDataSetCloner{T}"/> bypassing <see cref="ICopyable{T}"/> constraint.
		/// </summary>
		public static SetCloner CreateCopyingDataSetCloner<T>(DataSet<T> dataSet)
		{
			return (SetCloner)ReflectionUtils.CreateGeneric(typeof(CopyingDataSetCloner<>), typeof(T), dataSet);
		}
	}
}

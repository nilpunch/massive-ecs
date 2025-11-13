using System;
using System.Linq;
using Massive.AutoFree;

namespace Massive
{
	public static class AutoFreeUtils
	{
		public static bool IsImplementedFor(Type type)
		{
			return type.GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IAutoFree<>));
		}

		/// <summary>
		/// Create <see cref="AutoFreeDataSet{T}"/> bypassing <see cref="IAutoFree{T}"/> constraint.
		/// </summary>
		public static DataSet<T> CreateAutoFreeDataSet<T>(Allocator allocator, T defaultValue = default)
		{
			return (DataSet<T>)ReflectionUtils.CreateGeneric(typeof(AutoFreeDataSet<>), typeof(T), allocator, defaultValue);
		}
	}
}

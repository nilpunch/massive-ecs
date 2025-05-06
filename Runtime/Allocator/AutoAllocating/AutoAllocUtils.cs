using System;
using System.Linq;

namespace Massive
{
	public static class AutoAllocUtils
	{
		public static bool IsImplementedFor(Type type)
		{
			return type.GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IAutoAlloc<>));
		}

		/// <summary>
		/// Create <see cref="AutoAllocatingDataSet{T}"/> bypassing <see cref="IAutoAlloc{T}"/> constraint.
		/// </summary>
		public static DataSet<T> CreateAutoAllocatingDataSet<T>(AutoAllocConfig[] allocConfigs, int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous, T defaultValue = default)
		{
			return (DataSet<T>)ReflectionUtils.CreateGeneric(typeof(AutoAllocatingDataSet<>), typeof(T), allocConfigs, pageSize, packing, defaultValue);
		}
	}
}

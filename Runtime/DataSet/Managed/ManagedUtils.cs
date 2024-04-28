using System;
using System.Linq;

namespace Massive
{
	public static class ManagedUtils
	{
		public static bool IsManaged<T>()
		{
			return typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IManaged<>))
				.Any(x => x.GetGenericTypeDefinition().MakeGenericType(typeof(T)).IsAssignableFrom(typeof(T)));
		}

		/// <summary>
		/// Create <see cref="Massive.MassiveManagedDataSet{T}"/> bypassing <see cref="Massive.IManaged{T}"/> constraint.
		/// </summary>
		public static IDataSet<T> CreateMassiveManagedDataSet<T>(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
		{
			Type constructedType = typeof(MassiveManagedDataSet<>)
				.GetGenericTypeDefinition()
				.MakeGenericType(typeof(T));

			return (IDataSet<T>)Activator.CreateInstance(constructedType, dataCapacity, framesCapacity);
		}
	}
}

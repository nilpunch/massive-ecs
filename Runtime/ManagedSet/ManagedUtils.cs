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
		/// Create <see cref="Massive.ManagedDataSet{T}"/> bypassing <see cref="Massive.IManaged{T}"/> constraint.
		/// </summary>
		public static ISet CreateManagedDataSet<T>(int dataCapacity = Constants.DataCapacity) where T : struct
		{
			Type constructedType = typeof(ManagedDataSet<>)
				.GetGenericTypeDefinition()
				.MakeGenericType(typeof(T));

			return (ISet)Activator.CreateInstance(constructedType, dataCapacity);
		}

		/// <summary>
		/// Create <see cref="Massive.MassiveManagedDataSet{T}"/> bypassing <see cref="Massive.IManaged{T}"/> constraint.
		/// </summary>
		public static ISet CreateMassiveManagedDataSet<T>(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity) where T : struct
		{
			Type constructedType = typeof(MassiveManagedDataSet<>)
				.GetGenericTypeDefinition()
				.MakeGenericType(typeof(T));

			return (ISet)Activator.CreateInstance(constructedType, dataCapacity, framesCapacity);
		}
	}
}
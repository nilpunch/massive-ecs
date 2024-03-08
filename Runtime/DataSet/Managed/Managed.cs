using System;
using UnityEngine;

namespace Massive
{
	public static class Managed
	{
		public static ISet CreateDataSet<T>(int dataCapacity = Constants.DataCapacity) where T : struct
		{
			Type genericClassType = typeof(ManagedDataSet<>).GetGenericTypeDefinition();

			Type constructedType = genericClassType.MakeGenericType(typeof(T));

			return (ISet)Activator.CreateInstance(constructedType, dataCapacity);
		}

		public static ISet CreateMassiveDataSet<T>(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity) where T : struct
		{
			Type genericClassType = typeof(MassiveManagedDataSet<>).GetGenericTypeDefinition();

			Type constructedType = genericClassType.MakeGenericType(typeof(T));

			return (ISet)Activator.CreateInstance(constructedType, framesCapacity, dataCapacity);
		}
	}
}
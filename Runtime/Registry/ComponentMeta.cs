using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Massive
{
	/// <summary>
	/// Cross-platform component information.
	/// </summary>
	public static class ComponentMeta<T> where T : struct
	{
		public static bool HasAnyFields { get; }

		public static bool IsManaged { get; }

		static ComponentMeta()
		{
			HasAnyFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length > 0;

			IsManaged = typeof(T).GetInterfaces()
				.Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IManaged<>))
				.Any(x => x.GetGenericTypeDefinition().MakeGenericType(typeof(T)).IsAssignableFrom(typeof(T)));

			if (IsManaged)
			{
				Debug.Log(typeof(T).Name);
			}
		}

#if UNITY_2020_3_OR_NEWER
		[UnityEngine.Scripting.Preserve]
#endif
		static void VirtualGenericsHack()
		{
			new NormalSetFactory().CreateDataSet<T>();
			new MassiveSetFactory().CreateDataSet<T>();
		}
	}
}
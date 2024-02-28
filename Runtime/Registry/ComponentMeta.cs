﻿using System.Reflection;

namespace Massive.ECS
{
	/// <summary>
	/// Cross-platform component information.
	/// </summary>
	public static class ComponentMeta<T> where T : unmanaged
	{
		public static int SizeInBytes { get; }
		public static bool HasAnyFields { get; }

		static ComponentMeta()
		{
			unsafe
			{
				SizeInBytes = sizeof(T);
				HasAnyFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length > 0;
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
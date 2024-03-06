using System;
using System.Reflection;

namespace Massive.ECS
{
	/// <summary>
	/// Cross-platform component information.
	/// </summary>
	public static class ComponentMeta<T> where T : struct
	{
		public static bool HasAnyFields { get; }

		public static bool IsManaged { get; }

		public static InitializeManaged<T> Initialize { get; }
		public static ResetManaged<T> Reset { get; }
		public static CloneManaged<T> Clone { get; }

		static ComponentMeta()
		{
			HasAnyFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length > 0;

			IsManaged = typeof(IManaged<T>).IsAssignableFrom(typeof(T));

			if (IsManaged)
			{
				var initializeMethod = typeof(T).GetMethod(nameof(IManaged<T>.Initialize), BindingFlags.Public | BindingFlags.Instance);
				var resetMethod = typeof(T).GetMethod(nameof(IManaged<T>.Reset), BindingFlags.Public | BindingFlags.Instance);
				var cloneMethod = typeof(T).GetMethod(nameof(IManaged<T>.Clone), BindingFlags.Public | BindingFlags.Instance);

				Initialize = (InitializeManaged<T>)Delegate.CreateDelegate(
					typeof(InitializeManaged<T>), _unusedInstance, initializeMethod!);

				Reset = (ResetManaged<T>)Delegate.CreateDelegate(
					typeof(ResetManaged<T>), _unusedInstance, resetMethod!);

				Clone = (CloneManaged<T>)Delegate.CreateDelegate(
					typeof(CloneManaged<T>), _unusedInstance, cloneMethod!);
			}
			else
			{
				Initialize = (out T data) => data = default;
				Reset = (ref T data) => data = default;
				Clone = (in T source, ref T destination) => destination = source;
			}
		}

		static readonly T _unusedInstance = default;

#if UNITY_2020_3_OR_NEWER
		[UnityEngine.Scripting.Preserve]
#endif
		static void VirtualGenericsHack()
		{
			new NormalSetFactory().CreateDataSet<T>();
			new MassiveSetFactory().CreateDataSet<T>();
			if (_unusedInstance is IManaged<T> managed)
			{
				managed.Initialize(out var value);
				managed.Reset(ref value);
				managed.Clone(value, ref value);
			}
		}
	}
}
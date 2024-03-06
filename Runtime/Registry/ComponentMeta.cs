using System;
using System.Linq;
using System.Reflection;

namespace Massive
{
	public delegate void InitializeManaged<T>(out T data) where T : struct;
	public delegate void ResetManaged<T>(ref T data) where T : struct;
	public delegate void CloneManaged<T>(in T source, ref T destination) where T : struct;
	
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

			IsManaged = typeof(IManaged<T>).IsAssignableFrom(typeof(T));
		}

#if UNITY_2020_3_OR_NEWER
		[UnityEngine.Scripting.Preserve]
#endif
		static void VirtualGenericsHack()
		{
			new NormalSetFactory().CreateDataSet<T>();
			new MassiveSetFactory().CreateDataSet<T>();
			if (_managedInstance is IManaged<T> managed)
			{
				var value = new T();
				managed.Initialize();
				managed.Reset();
				managed.CopyTo(ref value);
			}
		}

		static readonly T _managedInstance = default;

		private static TDelegate CreateDelegateFromManagedMethod<TDelegate>(string methodName) where TDelegate : Delegate
		{
			return (TDelegate)Delegate.CreateDelegate(typeof(TDelegate), _managedInstance, GetManagedMethod(methodName));
		}

		private static MethodInfo GetManagedMethod(string methodName)
		{
			var implicitMethod = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
			var explicitMethod = typeof(T).GetMethod($"{GetFullTypeName(typeof(IManaged<T>))}.{methodName}", BindingFlags.NonPublic | BindingFlags.Instance);
			return implicitMethod ?? explicitMethod;
		}

		private static string GetFullTypeName(Type type)
		{
			if(type.IsGenericType)
			{
				string genericArguments = string.Join(",", type.GetGenericArguments().Select(GetFullTypeName));
				string typeItself = type.FullName!.Substring(0, type.FullName.IndexOf("`", StringComparison.Ordinal));
				return $"{typeItself}<{genericArguments}>";
			}
			return type.FullName;
		}
	}
}
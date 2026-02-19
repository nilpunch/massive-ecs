using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Massive.AutoFree;
using Preserve = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
using Member = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Massive
{
	public static class AutoFreeUtils
	{
		private const string FactoryMethodName = "CreateDataSetAndCloner";

		public static bool IsImplementedFor([Preserve(Member.Interfaces)] Type type)
		{
			return type.GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IAutoFree<>));
		}

		/// <summary>
		/// Create <see cref="AutoFreeDataSet{T}"/> bypassing <see cref="IAutoFree{T}"/> constraint.
		/// </summary>
		[UnconditionalSuppressMessage("", "IL2072")]
		public static SetAndCloner CreateAutoFreeDataSet([Preserve(Member.Interfaces)] Type type, Allocator allocator, object defaultValue)
		{
			var autoFreeInterface = type.GetInterfaces()
				.First(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IAutoFree<>));

			return (SetAndCloner)CreateDataSetAndCloner(autoFreeInterface).Invoke(null, new object[] { allocator, defaultValue });
		}

		[DynamicDependency(FactoryMethodName, typeof(IAutoFree<>))]
		private static MethodInfo CreateDataSetAndCloner([Preserve(Member.PublicMethods)] Type type)
		{
			return type.GetMethod(FactoryMethodName, BindingFlags.Static | BindingFlags.Public);
		}
	}
}

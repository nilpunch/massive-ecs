using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Massive.AutoFree;
using Preserve = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
using Member = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Massive
{
	public static class AutoUtils
	{
		private const string FactoryMethodName = "CreateDataSetAndCloner";

		public static bool IsImplementedFor([Preserve(Member.Interfaces)] Type type)
		{
			return type.GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IAuto<>));
		}

		/// <summary>
		/// Create <see cref="AutoDataSet{T}"/> bypassing <see cref="IAuto{T}"/> constraint.
		/// </summary>
		[UnconditionalSuppressMessage("", "IL2072")]
		public static SetAndCloner CreateAutoDataSet([Preserve(Member.Interfaces)] Type type, Allocator allocator, object defaultValue)
		{
			var autoInterface = type.GetInterfaces()
				.First(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(IAuto<>));

			return (SetAndCloner)CreateDataSetAndCloner(autoInterface).Invoke(null, new object[] { allocator, defaultValue });
		}

		[DynamicDependency(FactoryMethodName, typeof(IAuto<>))]
		private static MethodInfo CreateDataSetAndCloner([Preserve(Member.PublicMethods)] Type type)
		{
			return type.GetMethod(FactoryMethodName, BindingFlags.Static | BindingFlags.Public);
		}
	}
}

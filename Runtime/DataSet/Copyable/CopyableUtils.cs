using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Preserve = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
using Member = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Massive
{
	public static class CopyableUtils
	{
		private const string FactoryMethodName = "CreateDataSetAndCloner";

		public static bool IsImplementedFor([Preserve(Member.Interfaces)] Type type)
		{
			return type.GetInterfaces()
				.Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICopyable<>));
		}

		/// <summary>
		/// Create <see cref="CopyingDataSet{T}"/> bypassing <see cref="ICopyable{T}"/> constraint.
		/// </summary>
		[UnconditionalSuppressMessage("", "IL2072")]
		public static SetAndCloner CreateCopyingDataSet([Preserve(Member.Interfaces)] Type type, object defaultValue)
		{
			var copyableInterface = type.GetInterfaces()
				.First(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICopyable<>));

			return (SetAndCloner)CreateDataSetAndCloner(copyableInterface).Invoke(null, new object[] { defaultValue });
		}

		[DynamicDependency(FactoryMethodName, typeof(ICopyable<>))]
		private static MethodInfo CreateDataSetAndCloner([Preserve(Member.PublicMethods)] Type type)
		{
			return type.GetMethod(FactoryMethodName, BindingFlags.Static | BindingFlags.Public);
		}
	}
}

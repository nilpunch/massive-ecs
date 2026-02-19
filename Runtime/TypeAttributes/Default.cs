using System;
using System.Reflection;
using Preserve = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
using Member = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Massive
{
	public static class Default<T>
	{
		public static readonly T Value;

		static Default()
		{
			var type = typeof(T);

#pragma warning disable IL2087
			var defaultValue = Default.GetDefaultValue(type);
#pragma warning restore IL2087

			Value = defaultValue != null ? (T)defaultValue : default;
		}
	}

	internal static class Default
	{
		internal static object GetDefaultValue([Preserve(Member.PublicProperties | Member.NonPublicProperties)] Type type)
		{
			var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

			foreach (var property in type.GetProperties(flags))
			{
				if (property.IsDefined(typeof(DefaultValueAttribute), inherit: false) &&
					property.PropertyType == type &&
					property.GetMethod != null)
				{
					return property.GetValue(null);
				}
			}

			return null;
		}
	}
}

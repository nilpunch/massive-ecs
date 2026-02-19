using System;
using System.Reflection;
using Preserve = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
using Member = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Massive
{
	public static class Default<[Preserve(Member.PublicProperties | Member.PublicMethods)] T>
	{
		public static readonly T Value;

		static Default()
		{
			var type = typeof(T);

			var defaultValue = Default.GetDefaultValue(type);

			Value = defaultValue != null ? (T)defaultValue : default;
		}
	}

	internal static class Default
	{
		internal static object GetDefaultValue([Preserve(Member.PublicProperties | Member.PublicMethods)] Type type)
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

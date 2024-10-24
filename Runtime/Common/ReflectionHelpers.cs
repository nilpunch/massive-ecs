using System;
using System.Linq;

namespace Massive
{
	public static class ReflectionHelpers
	{
		/// <summary>
		/// Returns full type name with namespace and generic arguments.
		/// </summary>
		public static string GetFullGenericName(this Type type)
		{
			if (type.IsGenericType)
			{
				string genericArguments = string.Join(',', type.GetGenericArguments().Select(GetFullGenericName));
				string typeItself = type.FullName[..type.FullName.IndexOf('`', StringComparison.Ordinal)];
				return $"{typeItself}<{genericArguments}>";
			}
			return type.FullName;
		}

		/// <summary>
		/// Returns type name with generic arguments.
		/// </summary>
		public static string GetGenericName(this Type type)
		{
			if (type.IsGenericType)
			{
				string genericArguments = string.Join(',', type.GetGenericArguments().Select(GetGenericName));
				string typeItself = type.Name[..type.Name.IndexOf('`', StringComparison.Ordinal)];
				return $"{typeItself}<{genericArguments}>";
			}
			return type.Name;
		}

		public static object CreateGeneric(Type genericType, Type genericArg, params object[] args)
		{
			var constructedType = genericType.MakeGenericType(genericArg);
			return Activator.CreateInstance(constructedType, args);
		}
	}
}

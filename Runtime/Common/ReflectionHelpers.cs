using System;
using System.Linq;

namespace Massive
{
	public static class ReflectionHelpers
	{
		/// <summary>
		/// Returns full type name with namespace and generic arguments.
		/// </summary>
		public static string GetFullName(Type type)
		{
			if (type.IsGenericType)
			{
				string genericArguments = string.Join(',', type.GetGenericArguments().Select(GetFullName));
				string typeItself = type.FullName[..type.FullName.IndexOf('`', StringComparison.Ordinal)];
				return $"{typeItself}<{genericArguments}>";
			}
			return type.FullName;
		}
	}
}

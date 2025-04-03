using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Massive
{
	public static class ReflectionUtils
	{
		/// <summary>
		/// Returns full type name with namespace and generic arguments.
		/// </summary>
		public static string GetFullGenericName(this Type type)
		{
			if (type.IsGenericType)
			{
				var genericArguments = string.Join(',', type.GetGenericArguments().Select(GetFullGenericName));
				var typeItself = type.FullName[..type.FullName.IndexOf('`', StringComparison.Ordinal)];
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
				var genericArguments = string.Join(',', type.GetGenericArguments().Select(GetGenericName));
				var typeItself = type.Name[..type.Name.IndexOf('`', StringComparison.Ordinal)];
				return $"{typeItself}<{genericArguments}>";
			}
			return type.Name;
		}

		public static object CreateGeneric(Type genericType, Type genericArg, params object[] args)
		{
			var constructedType = genericType.MakeGenericType(genericArg);
			return Activator.CreateInstance(constructedType, args);
		}

		public static bool HasNoFields(Type type)
		{
			return !HasAnyFields(type);
		}

		public static bool HasAnyFields(Type type)
		{
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length > 0;
		}

		private static Dictionary<Type, bool> s_cachedTypes = new Dictionary<Type, bool>();

		public static bool IsManaged(this Type type)
		{
			return !IsUnmanaged(type);
		}

		public static bool IsUnmanaged(this Type t)
		{
			var result = false;
			if (s_cachedTypes.ContainsKey(t))
			{
				return s_cachedTypes[t];
			}
			else if (t.IsPrimitive || t.IsPointer || t.IsEnum)
			{
				result = true;
			}
			else if (t.IsGenericType || !t.IsValueType)
			{
				result = false;
			}
			else
			{
				result = t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
					.All(x => x.FieldType.IsUnmanaged());
			}
			s_cachedTypes.Add(t, result);
			return result;
		}

		private class ConstraintUnmanaged<T> where T : unmanaged
		{
		}
	}
}

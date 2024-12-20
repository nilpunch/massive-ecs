﻿using System;
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

		public static bool IsManaged(this Type type)
		{
			return !IsUnmanaged(type);
		}

		public static bool IsUnmanaged(this Type type)
		{
			try
			{
				// ReSharper disable once ReturnValueOfPureMethodIsNotUsed
				typeof(ConstraintUnmanaged<>).MakeGenericType(type);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private class ConstraintUnmanaged<T> where T : unmanaged
		{
		}
	}
}

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Preserve = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembersAttribute;
using Member = System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

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

		public static bool HasNoFields([Preserve(Member.PublicFields | Member.NonPublicFields)] Type type)
		{
			return !HasAnyFields(type);
		}

		public static bool HasAnyFields([Preserve(Member.PublicFields | Member.NonPublicFields)] Type type)
		{
			return type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Length > 0;
		}

		private static readonly Dictionary<Type, bool> s_managedCache = new Dictionary<Type, bool>();

		public static bool IsManaged([Preserve(Member.PublicFields | Member.NonPublicFields)] this Type type)
		{
			return !IsUnmanaged(type);
		}

		[UnconditionalSuppressMessage("", "IL2072")]
		public static bool IsUnmanaged([Preserve(Member.PublicFields | Member.NonPublicFields)] this Type type)
		{
			if (!s_managedCache.TryGetValue(type, out var isUnmanaged))
			{
				if (type.IsPrimitive || type.IsPointer || type.IsEnum)
				{
					isUnmanaged = true;
				}
				else if (!type.IsValueType)
				{
					isUnmanaged = false;
				}
				else
				{
					isUnmanaged = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
						.All(x => x.FieldType.IsUnmanaged());
				}
				s_managedCache.Add(type, isUnmanaged);
			}

			return isUnmanaged;
		}

#if NET5_0_OR_GREATER
		public static void PreserveSize<T>()
		{
			s_sizeOfCache[typeof(T)] = System.Runtime.CompilerServices.Unsafe.SizeOf<T>();
		}
#else
		public static void PreserveSize<T>()
		{
		}
#endif

		private static readonly Dictionary<Type, int> s_sizeOfCache = new Dictionary<Type, int>();

		private static unsafe int SizeOf<T>() where T : unmanaged => sizeof(T);

		public static int SizeOfUnmanaged(Type t)
		{
#if NET5_0_OR_GREATER
			if (!s_sizeOfCache.TryGetValue(t, out var size))
			{
				throw new Exception($"Can't get runtime size of {t.GetFullGenericName()}.");
			}

			return size;
#else
			if (!s_sizeOfCache.TryGetValue(t, out var size))
			{
				try
				{
					if (t.IsPointer)
					{
						size = IntPtr.Size;
					}
					else if (t.IsGenericType || t.IsByRef || t.IsArray || t.ContainsGenericParameters)
					{
						size = SizeOfGeneric(t);
					}
					else
					{
						size = Marshal.SizeOf(t);
					}
				}
				catch
				{
					throw new Exception($"Can't get runtime size of {t.GetFullGenericName()}.");
				}
				s_sizeOfCache.Add(t, size);
			}

			return size;
#endif
		}

		private static int SizeOfGeneric(Type t)
		{
			var genericMethod = typeof(ReflectionUtils)
				.GetMethod(nameof(SizeOf), BindingFlags.Static | BindingFlags.NonPublic)
				.MakeGenericMethod(t);
			var size = (int)genericMethod.Invoke(null, new object[] { });
			return size;
		}
	}
}

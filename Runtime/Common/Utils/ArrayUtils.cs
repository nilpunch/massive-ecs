using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class ArrayUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] Resize<T>(this T[] array, int capacity)
		{
			Array.Resize(ref array, capacity);
			return array;
		}

		public static bool Contains<T>(this T[] array, T value)
		{
			var equalityComparer = EqualityComparer<T>.Default;
			var length = array.Length;

			for (var i = 0; i < length; i++)
			{
				if (equalityComparer.Equals(array[i], value))
				{
					return true;
				}
			}

			return false;
		}

		public static bool ContainsAll<T>(this T[] array, T[] other)
		{
			for (var i = 0; i < other.Length; i++)
			{
				if (!array.Contains(other[i]))
				{
					return false;
				}
			}

			return true;
		}

		public static bool ContainsAny<T>(this T[] array, T[] other)
		{
			for (var i = 0; i < other.Length; i++)
			{
				if (array.Contains(other[i]))
				{
					return true;
				}
			}

			return false;
		}

		public static bool ContainsNo<T>(this T[] array, T[] other)
		{
			for (var i = 0; i < other.Length; i++)
			{
				if (array.Contains(other[i]))
				{
					return false;
				}
			}

			return true;
		}

		public static int GetUnorderedHashCode<T>(this T[] array)
		{
			var hash = 0;
			for (var i = 0; i < array.Length; i++)
			{
				hash ^= array[i].GetHashCode();
			}
			return hash;
		}
	}
}

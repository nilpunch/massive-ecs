using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class ArrayUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] Resize<T>(this T[] array, int capacity)
		{
			Array.Resize(ref array, capacity);
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ResizeToNextPowOf2<T>(this T[] array, int capacity)
		{
			Array.Resize(ref array, MathUtils.RoundUpToPowerOfTwo(capacity));
			return array;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static T[] ResizeToNextPowOf2<T>(this T[] array, int capacity, T growFill)
		{
			var lastCapacity = array.Length;
			Array.Resize(ref array, MathUtils.RoundUpToPowerOfTwo(capacity));
			if (array.Length > lastCapacity)
			{
				Array.Fill(array, growFill, lastCapacity, array.Length - lastCapacity);
			}
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

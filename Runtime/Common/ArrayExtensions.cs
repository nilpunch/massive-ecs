using System;

namespace Massive
{
	public static class ArrayExtensions
	{
		public static bool Contains<T>(this T[] array, T value)
		{
			return Array.IndexOf(array, value) != -1;
		}

		public static bool Contains<T>(this T[] array, T[] other)
		{
			for (int i = 0; i < other.Length; i++)
			{
				if (!array.Contains(other[i]))
				{
					return false;
				}
			}

			return true;
		}

		public static int GetUnorderedHashCode<T>(this T[] array)
		{
			int hash = 0;
			for (var i = 0; i < array.Length; i++)
			{
				hash ^= array[i].GetHashCode();
			}
			return hash;
		}
	}
}
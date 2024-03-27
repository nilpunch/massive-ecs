using System;

namespace Massive
{
	public static class ArrayExtensions
	{
		public static bool Contains<T>(this T[] array, T value)
		{
			return Array.IndexOf(array, value) != -1;
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

		public static bool IsSubsetOf<T>(this T[] array, T[] other)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (!other.Contains(array[i]))
				{
					return false;
				}
			}

			return true;
		}
	}
}
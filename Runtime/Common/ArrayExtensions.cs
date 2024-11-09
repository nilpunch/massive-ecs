using System.Collections.Generic;

namespace Massive
{
	public static class ArrayExtensions
	{
		public static bool Contains<T>(this T[] array, T value)
		{
			var equalityComparer = EqualityComparer<T>.Default;
			int length = array.Length;

			for (int i = 0; i < length; i++)
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
			for (int i = 0; i < other.Length; i++)
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
			for (int i = 0; i < other.Length; i++)
			{
				if (array.Contains(other[i]))
				{
					return true;
				}
			}

			return false;
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

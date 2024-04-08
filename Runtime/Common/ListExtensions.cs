using System.Collections.Generic;

namespace Massive
{
	public static class ListExtensions
	{
		public static bool Contains<T>(this IReadOnlyList<T> list, T value)
		{
			var equalityComparer = EqualityComparer<T>.Default;
			int count = list.Count;

			for (int i = 0; i < count; i++)
			{
				if (equalityComparer.Equals(list[i], value))
				{
					return true;
				}
			}

			return false;
		}

		public static bool Contains<T>(this IReadOnlyList<T> array, IReadOnlyList<T> other)
		{
			for (int i = 0; i < other.Count; i++)
			{
				if (!array.Contains(other[i]))
				{
					return false;
				}
			}

			return true;
		}

		public static int GetUnorderedHashCode<T>(this IReadOnlyList<T> list)
		{
			int hash = 0;
			for (var i = 0; i < list.Count; i++)
			{
				hash ^= list[i].GetHashCode();
			}
			return hash;
		}
	}
}
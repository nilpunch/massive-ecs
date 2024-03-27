using System;

namespace Massive
{
	public static class ArrayExtensions
	{
		public static bool Contains<T>(this T[] array, T value)
		{
			return Array.IndexOf(array, value) != -1;
		}
	}
}
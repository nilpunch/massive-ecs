using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class DataSetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set<T>(this DataSet<T> dataSet, int id, T data)
		{
			dataSet.Add(id);
			dataSet.Get(id) = data;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DataSet<T> Clone<T>(this DataSet<T> dataSet)
		{
			var clone = new DataSet<T>();
			dataSet.CopyTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopyTo<T>(this DataSet<T> source, DataSet<T> destination)
		{
			source.CopySparseTo(destination);

			var sourceData = source.Data;
			var destinationData = source.Data;

			foreach (var page in new PageSequence(sourceData.PageSize, source.Count))
			{
				if (!sourceData.HasPage(page.Index))
				{
					continue;
				}

				destinationData.EnsurePage(page.Index);

				var sourcePage = sourceData.Pages[page.Index];
				var destinationPage = destinationData.Pages[page.Index];

				Array.Copy(sourcePage, destinationPage, page.Length);
			}
		}
	}
}

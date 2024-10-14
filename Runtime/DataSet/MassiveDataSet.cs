using System;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveDataSet<T> : MassiveDataSetBase<T>
	{
		public MassiveDataSet(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			int pageSize = Constants.DefaultPageSize) : base(setCapacity, framesCapacity, pageSize)
		{
		}

		protected override void CopyData(PagedArray<T> source, PagedArray<T> destination, int count)
		{
			foreach (var (pageIndex, pageLength, _) in new PageSequence(source.PageSize, count))
			{
				if (!source.HasPage(pageIndex))
				{
					continue;
				}

				destination.EnsurePage(pageIndex);

				var sourcePage = source.Pages[pageIndex];
				var destinationPage = destination.Pages[pageIndex];

				Array.Copy(sourcePage, destinationPage, pageLength);
			}
		}
	}
}

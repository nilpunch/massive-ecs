using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/> with managed data support.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class MassiveManagedDataSet<T> : MassiveDataSetBase<T> where T : IManaged<T>
	{
		public MassiveManagedDataSet(int capacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			int pageSize = Constants.DefaultPageSize, IndexingMode indexingMode = IndexingMode.Packed)
			: base(capacity, framesCapacity, pageSize, indexingMode)
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

				for (int i = 0; i < pageLength; i++)
				{
					sourcePage[i].CopyTo(ref destinationPage[i]);
				}
			}
		}
	}
}

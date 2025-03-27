using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.DataSet{T}"/> with custom copying.
	/// Swaps data when elements are moved.
	/// Used for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class MassiveCopyingDataSet<T> : MassiveSwappingDataSet<T> where T : ICopyable<T>
	{
		public MassiveCopyingDataSet(int framesCapacity = Constants.DefaultFramesCapacity,
			int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(framesCapacity, pageSize, packing)
		{
		}

		public override void CopyDataAt(int source, int destination)
		{
			Data[source].CopyTo(ref Data[destination]);
		}

		protected override void CopyData(PagedArray<T> source, PagedArray<T> destination, int count)
		{
			foreach (var page in new PageSequence(source.PageSize, count))
			{
				if (!source.HasPage(page.Index))
				{
					continue;
				}

				destination.EnsurePage(page.Index);

				var sourcePage = source.Pages[page.Index];
				var destinationPage = destination.Pages[page.Index];

				for (var i = 0; i < page.Length; i++)
				{
					sourcePage[i].CopyTo(ref destinationPage[i]);
				}
			}
		}
	}
}

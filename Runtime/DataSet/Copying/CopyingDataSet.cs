using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/> with custom copying.
	/// Swaps data when elements are moved.
	/// Used for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class CopyingDataSet<T> : SwappingDataSet<T> where T : ICopyable<T>
	{
		public CopyingDataSet(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(pageSize, packing)
		{
		}

		public override void CopyDataAt(int source, int destination)
		{
			Data[source].CopyTo(ref Data[destination]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CopyingDataSet<T> CloneCopyable()
		{
			var clone = new CopyingDataSet<T>();
			CopyTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToCopyable(DataSet<T> other)
		{
			CopySparseTo(other);

			var sourceData = Data;
			var destinationData = other.Data;

			foreach (var page in new PageSequence(sourceData.PageSize, Count))
			{
				if (!sourceData.HasPage(page.Index))
				{
					continue;
				}

				destinationData.EnsurePage(page.Index);

				var sourcePage = sourceData.Pages[page.Index];
				var destinationPage = destinationData.Pages[page.Index];

				for (var i = 0; i < page.Length; i++)
				{
					sourcePage[i].CopyTo(ref destinationPage[i]);
				}
			}
		}
	}
}

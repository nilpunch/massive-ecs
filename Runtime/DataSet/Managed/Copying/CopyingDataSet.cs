using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Copying extension for <see cref="Massive.ManagedDataSet{T}"/>.
	/// Swaps data when elements are moved.
	/// Used for managed components to reduce allocations.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class CopyingDataSet<T> : ManagedDataSet<T> where T : ICopyable<T>
	{
		public CopyingDataSet(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(pageSize, packing)
		{
		}

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public override void CopyDataAt(int source, int destination)
		{
			Data[source].CopyTo(ref Data[destination]);
		}

		/// <summary>
		/// Creates and returns a new copying data set that is an exact copy of this one.
		/// All data is copied using <see cref="ICopyable{T}"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CopyingDataSet<T> CloneCopyable()
		{
			var clone = new CopyingDataSet<T>(Data.PageSize);
			CopyToCopyable(clone);
			return clone;
		}

		/// <summary>
		/// Copies all data and sparse state from this set into the specified one.
		/// All data is copied using <see cref="ICopyable{T}"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyToCopyable(DataSet<T> other)
		{
			IncompatiblePageSizeException.ThrowIfIncompatible(Data, other.Data);

			CopySparseTo(other);

			var sourceData = Data;
			var destinationData = other.Data;

			foreach (var page in new PageSequence(sourceData.PageSize, Count))
			{
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

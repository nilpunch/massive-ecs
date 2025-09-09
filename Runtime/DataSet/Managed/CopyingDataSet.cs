#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
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
	public class CopyingDataSet<T> : DataSet<T> where T : ICopyable<T>
	{
		public CopyingDataSet(int pageSize = Constants.DefaultPageSize)
			: base(pageSize)
		{
		}

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public override void CopyData(int sourceId, int destinationId)
		{
			Get(sourceId).CopyTo(ref Get(destinationId));
		}

		/// <summary>
		/// Creates and returns a new copying data set that is an exact copy of this one.
		/// All data is copied using <see cref="ICopyable{T}"/>.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CopyingDataSet<T> CloneCopyable()
		{
			var clone = new CopyingDataSet<T>(PageSize);
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
			IncompatiblePageSizeException.ThrowIfIncompatible(this, other);

			CopyBitsTo(other);

			foreach (var page in new PageSequence(PageSize, UsedBlocks << 6))
			{
				other.EnsurePage(page.Index);

				var sourcePage = PagedData[page.Index];
				var destinationPage = other.PagedData[page.Index];

				for (var i = 0; i < page.Length; i++)
				{
					sourcePage[i].CopyTo(ref destinationPage[i]);
				}
			}

			if (UsedBlocks > other.Blocks.Length)
			{
				other.Blocks = other.Blocks.ResizeToNextPowOf2(UsedBlocks);
			}

			Array.Copy(Blocks, other.Blocks, UsedBlocks);

			other.UsedBlocks = UsedBlocks;
			other.NextFreeBlock = NextFreeBlock;
		}
	}
}

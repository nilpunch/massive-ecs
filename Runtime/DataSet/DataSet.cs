#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// Does not reset the data for added elements.
	/// Does not preserve data when elements are moved.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DataSet<T> : SparseSet, IDataSet
	{
		/// <summary>
		/// The packed array that stores paged data.
		/// </summary>
		public PagedArray<T> Data { get; }

		public DataSet(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(packing)
		{
			Data = new PagedArray<T>(pageSize);
		}

		/// <summary>
		/// Gets a reference to the data associated with the specified ID.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			InvalidGetOperationException.ThrowIfNotAdded(this, id);

			return ref Data[Sparse[id]];
		}

		/// <summary>
		/// Adds the specified ID if not present and sets the associated data.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int id, T data)
		{
			NegativeArgumentException.ThrowIfNegative(id);

			EnsureSparseAt(id);

			var index = Sparse[id];
			if (index != Constants.InvalidId)
			{
				// If ID is already present, write the data.
				Data[index] = data;
				return;
			}

			if (Packing == Packing.WithHoles && NextHole != EndHole)
			{
				// Fill the hole.
				index = NextHole;
				NextHole = ~Packed[index];
			}
			else // Packing.Continuous || Packing.WithPersistentHoles
			{
				// Append to the end.
				index = Count;
				EnsurePackedAt(index);
				Data.EnsurePageAt(index);
				Count += 1;
			}

			Pair(id, index);
			Data[index] = data;

			UsedIds = MathUtils.Max(UsedIds, id + 1);

			NotifyAfterAdded(id);
		}

		/// <summary>
		/// Moves the data from one index to another.
		/// </summary>
		protected override void MoveDataAt(int source, int destination)
		{
			Data[destination] = Data[source];
		}

		/// <summary>
		/// Swaps the data between two indices.
		/// </summary>
		public override void SwapDataAt(int first, int second)
		{
			InvalidPackedIndexException.ThrowIfNotPacked(this, first);
			InvalidPackedIndexException.ThrowIfNotPacked(this, second);

			Data.Swap(first, second);
		}

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public override void CopyDataAt(int source, int destination)
		{
			InvalidPackedIndexException.ThrowIfNotPacked(this, source);
			InvalidPackedIndexException.ThrowIfNotPacked(this, destination);

			Data[destination] = Data[source];
		}

		/// <summary>
		/// Ensures data exists at the specified index.
		/// </summary>
		protected override void EnsureAndPrepareDataAt(int index)
		{
			Data.EnsurePageAt(index);
		}

		IPagedArray IDataSet.Data => Data;

		object IDataSet.GetRaw(int id) => Get(id);

		void IDataSet.SetRaw(int id, object value) => Set(id, (T)value);

		/// <summary>
		/// Creates and returns a new data set that is an exact copy of this one.
		/// All data is copied by value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DataSet<T> Clone()
		{
			var clone = new DataSet<T>(Data.PageSize);
			CopyTo(clone);
			return clone;
		}

		/// <summary>
		/// Copies all data and sparse state from this set into the specified one.
		/// All data is copied by value.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(DataSet<T> other)
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

				Array.Copy(sourcePage, destinationPage, page.Length);
			}
		}
	}
}

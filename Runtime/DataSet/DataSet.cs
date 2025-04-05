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
	/// Resets data when elemets are moved.
	/// Used for unmanaged components.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
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
			Assert.Has(this, id);

			return ref Data[Sparse[id]];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int id, T data)
		{
			Add(id);
			Data[Sparse[id]] = data;
		}

		/// <summary>
		/// Moves the data from one index to another.
		/// </summary>
		protected override void MoveDataAt(int source, int destination)
		{
			ref var sourceData = ref Data[source];
			Data[destination] = sourceData;
			sourceData = default;
		}

		/// <summary>
		/// Swaps the data between two indices.
		/// </summary>
		public override void SwapDataAt(int first, int second)
		{
			Assert.HasPacked(this, first);
			Assert.HasPacked(this, second);

			Data.Swap(first, second);
		}

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public override void CopyDataAt(int source, int destination)
		{
			Assert.HasPacked(this, source);
			Assert.HasPacked(this, destination);

			Data[destination] = Data[source];
		}

		/// <summary>
		/// Ensures data exists at the specified index.
		/// </summary>
		public override void EnsureDataAt(int index)
		{
			Data.EnsurePageAt(index);
		}

		IPagedArray IDataSet.Data => Data;

		object IDataSet.GetRaw(int id) => Get(id);

		void IDataSet.SetRaw(int id, object value) => Get(id) = (T)value;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DataSet<T> Clone()
		{
			var clone = new DataSet<T>();
			CopyTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyTo(DataSet<T> other)
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

				Array.Copy(sourcePage, destinationPage, page.Length);
			}
		}
	}
}

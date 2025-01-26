#if !MASSIVE_RELEASE
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// Resets data when elemets are moved.
	/// Used in registry for unmanaged components.
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
			Assert.IdAssigned(this, id);

			return ref Data[Sparse[id]];
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
			Assert.IdAssignedAt(this, first);
			Assert.IdAssignedAt(this, second);

			Data.Swap(first, second);
		}

		/// <summary>
		/// Copies the data from one index to another.
		/// </summary>
		public override void CopyDataAt(int source, int destination)
		{
			Assert.IdAssignedAt(this, source);
			Assert.IdAssignedAt(this, destination);

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
	}
}

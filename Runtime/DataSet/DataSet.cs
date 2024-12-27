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
		public PagedArray<T> Data { get; }

		public DataSet(int pageSize = Constants.DefaultPageSize, Packing packing = Packing.Continuous)
			: base(packing)
		{
			Data = new PagedArray<T>(pageSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref Data[Sparse[id]];
		}

		public override void MoveDataAt(int source, int destination)
		{
			ref var sourceData = ref Data[source];
			Data[destination] = sourceData;
			sourceData = default;
		}

		public override void SwapDataAt(int first, int second)
		{
			Data.Swap(first, second);
		}

		public override void CopyDataAt(int source, int destination)
		{
			Data[destination] = Data[source];
		}

		public override void EnsureDataAt(int index)
		{
			Data.EnsurePageAt(index);
		}

		IPagedArray IDataSet.Data => Data;

		object IDataSet.GetRaw(int id) => Get(id);

		void IDataSet.SetRaw(int id, object value) => Get(id) = (T)value;
	}
}

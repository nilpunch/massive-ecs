using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class DataSet<T> : SparseSet, IDataSet<T>
	{
		public PagedArray<T> Data { get; }

		public DataSet(int setCapacity = Constants.DefaultCapacity, int pageSize = Constants.DefaultPageSize, IndexingMode indexingMode = IndexingMode.Packed)
			: base(setCapacity, indexingMode)
		{
			Data = new PagedArray<T>(pageSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void CopyData(int sourceId, int targetId)
		{
			Get(targetId) = Get(sourceId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref Data[Sparse[id]];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SwapPacked(int packedA, int packedB)
		{
			base.SwapPacked(packedA, packedB);
			Data.Swap(packedA, packedB);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CopyFromToPacked(int source, int destination)
		{
			base.CopyFromToPacked(source, destination);
			Data[destination] = Data[source];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void EnsureDataForIndex(int index)
		{
			Data.EnsurePageForIndex(index);
		}
	}
}

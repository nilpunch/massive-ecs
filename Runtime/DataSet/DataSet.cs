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

		public DataSet(int setCapacity = Constants.DefaultSetCapacity, int pageSize = Constants.DefaultPageSize)
			: base(setCapacity)
		{
			Data = new PagedArray<T>(pageSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Assign(int id)
		{
			Data.EnsurePageForIndex(Count);
			base.Assign(id);
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
		public override void SwapDense(int denseA, int denseB)
		{
			base.SwapDense(denseA, denseB);
			Data.Swap(denseA, denseB);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CopyFromToDense(int source, int destination)
		{
			base.CopyFromToDense(source, destination);
			Data[destination] = Data[source];
		}
	}
}

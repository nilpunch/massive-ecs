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

		public DataSet(int setCapacity = Constants.DefaultCapacity, int pageSize = Constants.DefaultPageSize, bool inPlace = false)
			: base(setCapacity, inPlace)
		{
			Data = new PagedArray<T>(pageSize);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Assign(int id)
		{
			if (id < 0)
			{
				return;
			}

			Data.EnsurePageForIndex(InPlace ? id : Count);
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
			return ref InPlace ? ref Data[id] : ref Data[Sparse[id]];
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

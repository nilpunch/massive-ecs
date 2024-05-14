using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class DataSet<T> : SparseSet, IDataSet<T>
	{
		private PackedArray<T> _data;

		public PackedArray<T> RawData => _data;

		public DataSet(int dataCapacity = Constants.DataCapacity)
			: base(dataCapacity)
		{
			_data = new PackedArray<T>();
		}

		public PackedSpan<T> Data => new PackedSpan<T>(RawData, Count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Assign(int id)
		{
			RawData.EnsurePageForIndex(Count);
			base.Assign(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(int id, T data)
		{
			RawData.EnsurePageForIndex(Count);
			base.Assign(id);
			RawData[Sparse[id]] = data;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref RawData[Sparse[id]];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SwapDense(int denseA, int denseB)
		{
			base.SwapDense(denseA, denseB);
			(RawData[denseA], RawData[denseB]) = (RawData[denseB], RawData[denseA]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CopyFromToDense(int source, int destination)
		{
			base.CopyFromToDense(source, destination);
			RawData[destination] = RawData[source];
		}
	}
}

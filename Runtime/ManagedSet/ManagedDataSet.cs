using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for any <see cref="Massive.ISet"/> with managed data support.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class ManagedDataSet<T> : SparseSet, IDataSet<T> where T : struct, IManaged<T>
	{
		private T _swapBuffer;
		private T[] _data;

		public T[] RawData => _data;

		public ManagedDataSet(int dataCapacity = Constants.DataCapacity)
			: base(dataCapacity)
		{
			_data = new T[DenseCapacity];
		}

		public Span<T> Data => new Span<T>(RawData, 0, Count);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Assign(int id, T data)
		{
			base.Assign(id);
			data.CopyTo(ref RawData[Sparse[id]]);
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
			RawData[denseA].CopyTo(ref _swapBuffer);
			RawData[denseB].CopyTo(ref RawData[denseA]);
			_swapBuffer.CopyTo(ref RawData[denseB]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void ResizeDense(int capacity)
		{
			base.ResizeDense(capacity);
			Array.Resize(ref _data, capacity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CopyFromToDense(int source, int destination)
		{
			base.CopyFromToDense(source, destination);
			RawData[source].CopyTo(ref RawData[destination]);
		}
	}
}
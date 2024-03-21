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
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class ManagedDataSet<T> : SparseSet, IDataSet<T> where T : struct, IManaged<T>
	{
		private T _swapBuffer;

		public T[] Data { get; }

		public ManagedDataSet(int dataCapacity = Constants.DataCapacity)
			: base(dataCapacity)
		{
			Data = new T[Dense.Length];

			for (int i = 0; i < Data.Length; i++)
			{
				Data[i].Initialize();
			}

			_swapBuffer.Initialize();
		}

		public Span<T> AliveData => new Span<T>(Data, 0, AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Ensure(int id, T data)
		{
			var dense = base.Ensure(id);
			data.CopyTo(ref Data[dense]);
			return dense;
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
			Data[denseA].CopyTo(ref _swapBuffer);
			Data[denseB].CopyTo(ref Data[denseA]);
			_swapBuffer.CopyTo(ref Data[denseB]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void CopyDense(int source, int destination)
		{
			base.CopyDense(source, destination);
			Data[source].CopyTo(ref Data[destination]);
		}
	}
}
using System;
using System.Runtime.CompilerServices;
using Massive.ECS;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for any <see cref="Massive.ISet"/> with managed data support.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class ManagedDataSet<T> : IDataSet<T> where T : struct
	{
		private T _swapBuffer;

		public SparseSet SparseSet { get; }
		public T[] Data { get; }

		public ManagedDataSet(int dataCapacity = Constants.DataCapacity)
			: this(new SparseSet(dataCapacity))
		{
		}

		protected ManagedDataSet(SparseSet sparseSet)
		{
			SparseSet = sparseSet;
			Data = new T[sparseSet.Capacity];

			for (int i = 0; i < Data.Length; i++)
			{
				ComponentMeta<T>.Initialize(out Data[i]);
			}

			ComponentMeta<T>.Initialize(out _swapBuffer);
		}

		public int Capacity => SparseSet.Capacity;

		public int AliveCount => SparseSet.AliveCount;

		public ReadOnlySpan<int> AliveIds => SparseSet.AliveIds;

		public Span<T> AliveData => new Span<T>(Data, 0, SparseSet.AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id)
		{
			var createInfo = SparseSet.Ensure(id);
			ComponentMeta<T>.Reset(ref Data[createInfo.Dense]);
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id, T data)
		{
			var createInfo = SparseSet.Ensure(id);
			ComponentMeta<T>.Clone(data, ref Data[createInfo.Dense]);
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? Delete(int id)
		{
			var deleteInfo = SparseSet.Delete(id);
			if (deleteInfo.HasValue)
			{
				ComponentMeta<T>.Clone(Data[deleteInfo.Value.DenseSource], ref Data[deleteInfo.Value.DenseTarget]);
			}

			return deleteInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? DeleteDense(int denseIndex)
		{
			var deleteInfo = SparseSet.DeleteDense(denseIndex);
			if (deleteInfo.HasValue)
			{
				ComponentMeta<T>.Clone(in Data[deleteInfo.Value.DenseSource], ref Data[deleteInfo.Value.DenseTarget]);
			}

			return deleteInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref Data[SparseSet.GetDense(id)];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDense(int id)
		{
			return SparseSet.GetDense(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			return SparseSet.TryGetDense(id, out dense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return SparseSet.IsAlive(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SwapDense(int denseA, int denseB)
		{
			SparseSet.SwapDense(denseA, denseB);
			
			ComponentMeta<T>.Clone(Data[denseA], ref _swapBuffer);
			ComponentMeta<T>.Clone(Data[denseB], ref Data[denseA]);
			ComponentMeta<T>.Clone(_swapBuffer, ref Data[denseB]);
		}
	}
}
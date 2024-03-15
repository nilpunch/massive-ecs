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
	public class ManagedDataSet<T> : IDataSet<T> where T : struct, IManaged<T>
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
				Data[i].Initialize();
			}

			_swapBuffer.Initialize();
		}

		public int Capacity => SparseSet.Capacity;

		public int AliveCount => SparseSet.AliveCount;

		public ReadOnlySpan<int> AliveIds => SparseSet.AliveIds;

		public Span<T> AliveData => new Span<T>(Data, 0, SparseSet.AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EnsureInfo Ensure(int id)
		{
			var createInfo = SparseSet.Ensure(id);
			Data[createInfo.Dense].Reset();
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EnsureInfo Ensure(int id, T data)
		{
			var createInfo = SparseSet.Ensure(id);
			data.CopyTo(ref Data[createInfo.Dense]);
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? Delete(int id)
		{
			var deleteInfo = SparseSet.Delete(id);
			if (deleteInfo.HasValue)
			{
				Data[deleteInfo.Value.DenseSource].CopyTo(ref Data[deleteInfo.Value.DenseTarget]);
			}

			return deleteInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? DeleteDense(int denseIndex)
		{
			var deleteInfo = SparseSet.DeleteDense(denseIndex);
			if (deleteInfo.HasValue)
			{
				Data[deleteInfo.Value.DenseSource].CopyTo(ref Data[deleteInfo.Value.DenseTarget]);
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

			Data[denseA].CopyTo(ref _swapBuffer);
			Data[denseB].CopyTo(ref Data[denseA]);
			_swapBuffer.CopyTo(ref Data[denseB]);
		}
	}
}
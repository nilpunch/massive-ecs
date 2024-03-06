using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for any <see cref="Massive.ISet"/> with custom managed data support.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class ManagedDataSet<T> : IDataSet<T> where T : struct
	{
		public IManagedCloner<T> Cloner { get; }
		public SparseSet SparseSet { get; }
		public T[] Data { get; }

		public ManagedDataSet(int dataCapacity = Constants.DataCapacity, IManagedCloner<T> cloner = null)
			: this(new SparseSet(dataCapacity), cloner)
		{
		}

		protected ManagedDataSet(SparseSet sparseSet, IManagedCloner<T> cloner = null)
		{
			SparseSet = sparseSet;
			Data = new T[sparseSet.Capacity];
			Cloner = cloner ?? new DefaultManagedCloner<T>();

			for (int i = 0; i < Data.Length; i++)
			{
				Cloner.Initialize(out Data[i]);
			}
		}

		public int Capacity => SparseSet.Capacity;

		public int AliveCount => SparseSet.AliveCount;

		public ReadOnlySpan<int> AliveIds => SparseSet.AliveIds;

		public Span<T> AliveData => new Span<T>(Data, 0, SparseSet.AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id)
		{
			var createInfo = SparseSet.Ensure(id);
			Cloner.Reset(ref Data[createInfo.Dense]);
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id, T data)
		{
			var createInfo = SparseSet.Ensure(id);
			Cloner.Clone(data, ref Data[createInfo.Dense]);
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? Delete(int id)
		{
			var deleteInfo = SparseSet.Delete(id);
			if (deleteInfo.HasValue)
			{
				Cloner.Clone(Data[deleteInfo.Value.DenseSource], ref Data[deleteInfo.Value.DenseTarget]);
			}

			return deleteInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? DeleteDense(int denseIndex)
		{
			var deleteInfo = SparseSet.DeleteDense(denseIndex);
			if (deleteInfo.HasValue)
			{
				Cloner.Clone(in Data[deleteInfo.Value.DenseSource], ref Data[deleteInfo.Value.DenseTarget]);
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
			
			Cloner.Initialize(out var temp);
			Cloner.Clone(Data[denseA], ref temp);
			Cloner.Clone(Data[denseB], ref Data[denseA]);
			Cloner.Clone(temp, ref Data[denseB]);
		}
	}
}
using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for any <see cref="Massive.ISet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class DataSet<T> : IDataSet<T> where T : struct
	{
		public SparseSet SparseSet { get; }
		public T[] Data { get; }

		public DataSet(int dataCapacity = Constants.DataCapacity)
			: this(new SparseSet(dataCapacity)) { }

		protected DataSet(SparseSet sparseSet)
		{
			SparseSet = sparseSet;
			Data = new T[sparseSet.Capacity];
		}

		public int Capacity => SparseSet.Capacity;

		public int AliveCount => SparseSet.AliveCount;

		public ReadOnlySpan<int> AliveIds => SparseSet.AliveIds;

		public Span<T> AliveData => new Span<T>(Data, 0, SparseSet.AliveCount);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id)
		{
			var createInfo = SparseSet.Ensure(id);
			Data[createInfo.Dense] = default;
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id, T data)
		{
			var createInfo = SparseSet.Ensure(id);
			Data[createInfo.Dense] = data;
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? Delete(int id)
		{
			var deleteInfo = SparseSet.Delete(id);
			if (deleteInfo.HasValue)
			{
				Data[deleteInfo.Value.DenseTarget] = Data[deleteInfo.Value.DenseSource];
			}

			return deleteInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? DeleteDense(int denseIndex)
		{
			var deleteInfo = SparseSet.DeleteDense(denseIndex);
			if (deleteInfo.HasValue)
			{
				Data[deleteInfo.Value.DenseTarget] = Data[deleteInfo.Value.DenseSource];
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
			(Data[denseA], Data[denseB]) = (Data[denseB], Data[denseA]);
		}
	}
}
using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// </summary>
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class DataSet<T> : IDataSet<T> where T : struct
	{
		protected readonly SparseSet SparseSet;
		protected readonly T[] Data;

		public DataSet(int dataCapacity = Constants.DataCapacity)
		{
			SparseSet = new SparseSet(dataCapacity);
			Data = new T[dataCapacity];
		}

		protected DataSet(SparseSet sparseSet)
		{
			SparseSet = sparseSet;
			Data = new T[sparseSet.DenseCapacity];
		}

		public Span<T> AliveData => new Span<T>(Data, 0, SparseSet.AliveCount);

		public ReadOnlySpan<int> AliveIds => SparseSet.AliveIds;

		public int AliveCount => SparseSet.AliveCount;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Ensure(int id, T data = default)
		{
			var createInfo = SparseSet.Ensure(id);
			Data[createInfo.Dense] = data;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create(T data = default)
		{
			var createInfo = SparseSet.Create();
			Data[createInfo.Dense] = data;
			return createInfo.Id;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Delete(int id)
		{
			var deleteInfo = SparseSet.Delete(id);
			if (deleteInfo.HasValue)
			{
				Data[deleteInfo.Value.DenseSwapTarget] = Data[deleteInfo.Value.DenseSwapSource];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void DeleteDense(int denseIndex)
		{
			var deleteInfo = SparseSet.DeleteDense(denseIndex);
			if (deleteInfo.HasValue)
			{
				Data[deleteInfo.Value.DenseSwapTarget] = Data[deleteInfo.Value.DenseSwapSource];
			}
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
	}
}
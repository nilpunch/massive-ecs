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
	public class DataSetBase<TData, TSparseSet> : IDataSet<TData>
		where TData : struct
		where TSparseSet : ISparseSet
	{
		protected readonly TSparseSet SparseSet;
		protected readonly TData[] Data;

		protected DataSetBase(TSparseSet sparseSet)
		{
			SparseSet = sparseSet;
			Data = new TData[sparseSet.Capacity];
		}

		public Span<TData> AliveData => new Span<TData>(Data, 0, SparseSet.AliveCount);

		public ReadOnlySpan<int> AliveIds => SparseSet.AliveIds;

		public int Capacity => Data.Length;

		public int AliveCount => SparseSet.AliveCount;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Ensure(int id, TData data = default)
		{
			var createInfo = SparseSet.Ensure(id);
			Data[createInfo.Dense] = data;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int Create(TData data = default)
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
		public ref TData Get(int id)
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
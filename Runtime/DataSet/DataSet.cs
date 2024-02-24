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
	public class DataSet<T> : DataSet<T, SparseSet> where T : struct
	{
		public DataSet(int dataCapacity = Constants.DataCapacity) : base(new SparseSet(dataCapacity))
		{
		}
	}
	
	/// <summary>
	/// Data extension for <see cref="Massive.ISet"/>.
	/// </summary>
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class DataSet<T, TSparseSet> : IDataSet<T>
		where T : struct
		where TSparseSet : ISet
	{
		protected readonly TSparseSet SparseSet;
		protected readonly T[] Data;

		protected DataSet(TSparseSet sparseSet)
		{
			SparseSet = sparseSet;
			Data = new T[sparseSet.Capacity];
		}

		public Span<T> AliveData => new Span<T>(Data, 0, SparseSet.AliveCount);

		public ReadOnlySpan<int> AliveIds => SparseSet.AliveIds;

		public int Capacity => Data.Length;

		public int AliveCount => SparseSet.AliveCount;

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
		public CreateInfo Create()
		{
			var createInfo = SparseSet.Create();
			Data[createInfo.Dense] = default;
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Create(T data )
		{
			var createInfo = SparseSet.Create();
			Data[createInfo.Dense] = data;
			return createInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? Delete(int id)
		{
			var deleteInfo = SparseSet.Delete(id);
			if (deleteInfo.HasValue)
			{
				Data[deleteInfo.Value.DenseSwapTarget] = Data[deleteInfo.Value.DenseSwapSource];
			}
			return deleteInfo;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? DeleteDense(int denseIndex)
		{
			var deleteInfo = SparseSet.DeleteDense(denseIndex);
			if (deleteInfo.HasValue)
			{
				Data[deleteInfo.Value.DenseSwapTarget] = Data[deleteInfo.Value.DenseSwapSource];
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
	}
}
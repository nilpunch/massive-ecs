using System.Runtime.CompilerServices;

namespace Massive
{
	public struct DataSetStorage<T>
	{
		public readonly T[] Data;
		public SparseSetStorage SparseSet;

		public DataSetStorage(int dataCapacity = Constants.DataCapacity)
		{
			Data = new T[dataCapacity];
			SparseSet = new SparseSetStorage(dataCapacity);
		}

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
		public CreateInfo Create(T data)
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
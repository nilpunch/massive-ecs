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
	public class DataSet<T> : IDataSet<T>
		where T : unmanaged
	{
		protected DataSetStorage<T> Storage;

		public DataSet(int dataCapacity)
		{
			Storage = new DataSetStorage<T>(dataCapacity);
		}

		public Span<T> AliveData
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new Span<T>(Storage.Data, 0, Storage.SparseSet.AliveCount);
		}
		
		public int AliveCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Storage.SparseSet.AliveCount;
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Storage.SparseSet.Dense.Length;
		}

		public ReadOnlySpan<int> AliveIds
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ReadOnlySpan<int>(Storage.SparseSet.Dense, 0, Storage.SparseSet.AliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id)
		{
			return Storage.Ensure(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id, T data)
		{
			return Storage.Ensure(id, data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Create()
		{
			return Storage.Create();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Create(T data)
		{
			return Storage.Create(data);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? Delete(int id)
		{
			return Storage.Delete(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public DeleteInfo? DeleteDense(int dense)
		{
			return Storage.DeleteDense(dense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(int id)
		{
			return ref Storage.Get(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int GetDense(int id)
		{
			return Storage.GetDense(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetDense(int id, out int dense)
		{
			return Storage.TryGetDense(id, out dense);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool IsAlive(int id)
		{
			return Storage.IsAlive(id);
		}
	}
}
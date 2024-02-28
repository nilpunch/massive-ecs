using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SparseSet : ISet
	{
		protected SparseSetStorage Storage;

		public SparseSet(int dataCapacity = Constants.DataCapacity)
		{
			Storage = new SparseSetStorage(dataCapacity);
		}
		
		public int AliveCount
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Storage.AliveCount;
		}

		public int Capacity
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Storage.Dense.Length;
		}

		public ReadOnlySpan<int> AliveIds
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new ReadOnlySpan<int>(Storage.Dense, 0, Storage.AliveCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Ensure(int id)
		{
			return Storage.Ensure(id);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CreateInfo Create()
		{
			return Storage.Create();
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
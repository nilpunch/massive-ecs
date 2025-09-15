#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilteredWorld : IQueryable
	{
		public Query.Context Context { get; }

		public World World => Context.World;

		public Filter Filter => Context.Filter;

		public FilteredWorld(World world, Filter filter = null)
		{
			Context = new Query.Context()
			{
				World = world,
				Filter = filter ?? Filter.Empty
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			Context.ForEach(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T, TAction>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			Context.ForEach<T, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T1, T2, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			Context.ForEach<T1, T2, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T1, T2, T3, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			Context.ForEach<T1, T2, T3, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T1, T2, T3, T4, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			Context.ForEach<T1, T2, T3, T4, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitsEnumerator GetEnumerator()
		{
			var resultBitSet = RentAndPrepareBits(out var blocksLength);
			return new BitsEnumerator(resultBitSet, blocksLength);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerable Entities()
		{
			var resultBitSet = RentAndPrepareBits(out var blocksLength);
			return new EntityEnumerable(resultBitSet, World, blocksLength);
		}

		private BitSet RentAndPrepareBits(out int blocksLength)
		{
			var minBitSet = BitSetBase.GetMinBitSet(World.Entifiers, Filter.Included, Filter.IncludedCount);

			var resultBitSet = BitsPool.RentClone(minBitSet);

			if (minBitSet == World.Entifiers)
			{
				resultBitSet.RemoveOnRemove(World.Entifiers);
			}

			blocksLength = minBitSet.NonEmptyBlocks.Length;

			for (var i = 0; i < Filter.IncludedCount; i++)
			{
				var included = Filter.Included[i];
				resultBitSet.AndBits(included);
				resultBitSet.RemoveOnRemove(included);
			}

			for (var i = 0; i < Filter.ExcludedCount; i++)
			{
				var excluded = Filter.Excluded[i];
				resultBitSet.NotBits(excluded);
				resultBitSet.RemoveOnAdd(excluded);
			}

			return resultBitSet;
		}
	}
}

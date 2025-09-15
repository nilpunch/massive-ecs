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
			var resultBits = RentAndPrepareBits(out var bits1Length);
			return new BitsEnumerator(resultBits, bits1Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerable Entities()
		{
			var resultBits = RentAndPrepareBits(out var bits1Length);
			return new EntityEnumerable(resultBits, World, bits1Length);
		}

		private Bits RentAndPrepareBits(out int bits1Length)
		{
			var resultBits = BitsPool.Rent();

			if (Filter.IncludedCount == 0)
			{
				World.Entifiers.CopyBitsTo(resultBits);
				resultBits.RemoveOnRemove(World.Entifiers);
				bits1Length = World.Entifiers.Bits1.Length;
			}
			else
			{
				var minBits = BitsBase.GetMinBits(Filter.Included, Filter.IncludedCount);
				minBits.CopyBitsTo(resultBits);
				bits1Length = minBits.Bits1.Length;
			}

			for (var i = 0; i < Filter.IncludedCount; i++)
			{
				var included = Filter.Included[i];
				resultBits.AndBits(included);
				resultBits.RemoveOnRemove(included);
			}

			for (var i = 0; i < Filter.ExcludedCount; i++)
			{
				var excluded = Filter.Excluded[i];
				resultBits.NotBits(excluded);
				resultBits.RemoveOnAdd(excluded);
			}

			return resultBits;
		}
	}
}

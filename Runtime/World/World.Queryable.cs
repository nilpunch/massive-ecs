using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class World : IQueryable
	{
		Query.Context IQueryable.Context => Context;

		private Query.Context Context => new Query.Context()
		{
			World = this,
			Filter = Filter.Empty
		};

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
			var bits = BitsPool.RentClone(Entifiers).RemoveOnRemove(Entifiers);
			return new BitsEnumerator(bits, Entifiers.Bits1.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public EntityEnumerable Entities()
		{
			var bits = BitsPool.RentClone(Entifiers).RemoveOnRemove(Entifiers);
			return new EntityEnumerable(bits, this, Entifiers.Bits1.Length);
		}
	}
}

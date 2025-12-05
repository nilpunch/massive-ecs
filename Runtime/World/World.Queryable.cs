using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public partial class World : IQueryable
	{
		Query IQueryable.Query => Query;

		EntityEnumerable IQueryable.Entities => Query.Entities;

		private Query Query => new Query
		{
			World = this,
			Filter = Filter.Empty
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			Query.ForEach(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T, TAction>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			Query.ForEach<T, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T1, T2, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			Query.ForEach<T1, T2, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T1, T2, T3, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			Query.ForEach<T1, T2, T3, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T1, T2, T3, T4, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			Query.ForEach<T1, T2, T3, T4, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T1, T2, T3, T4, T5, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4, T5>
		{
			Query.ForEach<T1, T2, T3, T4, T5, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<T1, T2, T3, T4, T5, T6, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4, T5, T6>
		{
			Query.ForEach<T1, T2, T3, T4, T5, T6, TAction>(ref action);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IdsEnumerator GetEnumerator()
		{
			var bits = QueryCache.Rent().AddInclude(Entities).Update();
			return new IdsEnumerator(bits);
		}
	}
}

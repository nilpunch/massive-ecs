using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class QueryableExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable>(this TQueryable queryable, IdAction action)
			where TQueryable : IQueryable
		{
			var idActionAdapter = new IdActionAdapter { Action = action };
			queryable.Query.ForEach(ref idActionAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T>(this TQueryable queryable, IdActionRef<T> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T> { Action = action };
			queryable.Query.ForEach<T, IdActionRefAdapter<T>>(ref idActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2>(this TQueryable queryable, IdActionRef<T1, T2> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2> { Action = action };
			queryable.Query.ForEach<T1, T2, IdActionRefAdapter<T1, T2>>(ref idActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2, T3>(this TQueryable queryable, IdActionRef<T1, T2, T3> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2, T3> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, IdActionRefAdapter<T1, T2, T3>>(ref idActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2, T3, T4>(this TQueryable queryable, IdActionRef<T1, T2, T3, T4> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2, T3, T4> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, T4, IdActionRefAdapter<T1, T2, T3, T4>>(ref idActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs>(this TQueryable queryable, TArgs args, IdActionArgs<TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionArgsAdapter = new IdActionArgsAdapter<TArgs>() { Action = action, Args = args };
			queryable.Query.ForEach(ref idActionArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T>(this TQueryable queryable, TArgs args, IdActionRefArgs<T, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T, IdActionRefArgsAdapter<T, TArgs>>(ref idActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T1, T2>(this TQueryable queryable, TArgs args, IdActionRefArgs<T1, T2, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, IdActionRefArgsAdapter<T1, T2, TArgs>>(ref idActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T1, T2, T3>(this TQueryable queryable, TArgs args, IdActionRefArgs<T1, T2, T3, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, IdActionRefArgsAdapter<T1, T2, T3, TArgs>>(ref idActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4>(this TQueryable queryable, TArgs args, IdActionRefArgs<T1, T2, T3, T4, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, T3, T4, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, IdActionRefArgsAdapter<T1, T2, T3, T4, TArgs>>(ref idActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable>(this TQueryable queryable, EntityAction action)
			where TQueryable : IQueryable
		{
			var entityActionAdapter = new EntityActionAdapter { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World };
			queryable.Query.ForEach(ref entityActionAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T>(this TQueryable queryable, EntityActionRef<T> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T> { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World };
			queryable.Query.ForEach<T, EntityActionRefAdapter<T>>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2>(this TQueryable queryable, EntityActionRef<T1, T2> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2> { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World };
			queryable.Query.ForEach<T1, T2, EntityActionRefAdapter<T1, T2>>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2, T3>(this TQueryable queryable, EntityActionRef<T1, T2, T3> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3> { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World };
			queryable.Query.ForEach<T1, T2, T3, EntityActionRefAdapter<T1, T2, T3>>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2, T3, T4>(this TQueryable queryable, EntityActionRef<T1, T2, T3, T4> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3, T4> { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World };
			queryable.Query.ForEach<T1, T2, T3, T4, EntityActionRefAdapter<T1, T2, T3, T4>>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs>(this TQueryable queryable, TArgs args, EntityActionArgs<TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionArgsAdapter = new EntityActionArgsAdapter<TArgs>() { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World, Args = args };
			queryable.Query.ForEach(ref entityActionArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T, TArgs> { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World, Args = args };
			queryable.Query.ForEach<T, EntityActionRefArgsAdapter<T, TArgs>>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T1, T2>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T1, T2, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World, Args = args };
			queryable.Query.ForEach<T1, T2, EntityActionRefArgsAdapter<T1, T2, TArgs>>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T1, T2, T3>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T1, T2, T3, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World, Args = args };
			queryable.Query.ForEach<T1, T2, T3, EntityActionRefArgsAdapter<T1, T2, T3, TArgs>>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T1, T2, T3, T4, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs> { Action = action, Entifiers = queryable.World.Entifiers, World = queryable.World, Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs>>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T>(this TQueryable queryable, ActionRef<T> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T> { Action = action };
			queryable.Query.ForEach<T, ActionRefAdapter<T>>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2>(this TQueryable queryable, ActionRef<T1, T2> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2> { Action = action };
			queryable.Query.ForEach<T1, T2, ActionRefAdapter<T1, T2>>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2, T3>(this TQueryable queryable, ActionRef<T1, T2, T3> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, ActionRefAdapter<T1, T2, T3>>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, T1, T2, T3, T4>(this TQueryable queryable, ActionRef<T1, T2, T3, T4> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3, T4> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, T4, ActionRefAdapter<T1, T2, T3, T4>>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T>(this TQueryable queryable, TArgs args, ActionRefArgs<T, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T, ActionRefArgsAdapter<T, TArgs>>(ref actionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T1, T2>(this TQueryable queryable, TArgs args, ActionRefArgs<T1, T2, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, ActionRefArgsAdapter<T1, T2, TArgs>>(ref actionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TQueryable, TArgs, T1, T2, T3>(this TQueryable queryable, TArgs args, ActionRefArgs<T1, T2, T3, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, ActionRefArgsAdapter<T1, T2, T3, TArgs>>(ref actionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TQueryable>(this TQueryable queryable, IList<int> result)
			where TQueryable : IQueryable
		{
			var fillIds = new FillIds { Result = result };
			queryable.Query.ForEach(ref fillIds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TQueryable>(this TQueryable queryable, IList<Entifier> result)
			where TQueryable : IQueryable
		{
			var fillEntifiers = new FillEntifiers { Result = result, Entifiers = queryable.World.Entifiers };
			queryable.Query.ForEach(ref fillEntifiers);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int First<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			foreach (var id in queryable)
			{
				return id;
			}

			return Constants.InvalidId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity FirstEntity<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			foreach (var id in queryable)
			{
				return queryable.World.GetEntity(id);
			}

			return new Entity(Entifier.Dead, queryable.World);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			var destroyEntities = new DestroyAll { Entifiers = queryable.World.Entifiers };
			queryable.Query.ForEach(ref destroyEntities);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Count<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			var countEntities = new CountAll();
			queryable.Query.ForEach(ref countEntities);
			return countEntities.Result;
		}
	}
}

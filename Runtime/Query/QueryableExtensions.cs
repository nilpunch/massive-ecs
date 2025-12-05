using System.Collections.Generic;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppEagerStaticClassConstruction]
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class QueryableExtensions
	{
		public static void ForEach<TQueryable>(this TQueryable queryable, IdAction action)
			where TQueryable : IQueryable
		{
			var idActionAdapter = new IdActionAdapter { Action = action };
			queryable.Query.ForEach(ref idActionAdapter);
		}

		public static void ForEach<TQueryable, T>(this TQueryable queryable, IdActionRef<T> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T> { Action = action };
			queryable.Query.ForEach<T, IdActionRefAdapter<T>>(ref idActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2>(this TQueryable queryable, IdActionRef<T1, T2> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2> { Action = action };
			queryable.Query.ForEach<T1, T2, IdActionRefAdapter<T1, T2>>(ref idActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3>(this TQueryable queryable, IdActionRef<T1, T2, T3> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2, T3> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, IdActionRefAdapter<T1, T2, T3>>(ref idActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4>(this TQueryable queryable, IdActionRef<T1, T2, T3, T4> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2, T3, T4> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, T4, IdActionRefAdapter<T1, T2, T3, T4>>(ref idActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4, T5>(this TQueryable queryable, IdActionRef<T1, T2, T3, T4, T5> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2, T3, T4, T5> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, IdActionRefAdapter<T1, T2, T3, T4, T5>>(ref idActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4, T5, T6>(this TQueryable queryable, IdActionRef<T1, T2, T3, T4, T5, T6> action)
			where TQueryable : IQueryable
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2, T3, T4, T5, T6> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, T6, IdActionRefAdapter<T1, T2, T3, T4, T5, T6>>(ref idActionRefAdapter);
		}

		public static void ForEach<TQueryable, TArgs>(this TQueryable queryable, TArgs args, IdActionArgs<TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionArgsAdapter = new IdActionArgsAdapter<TArgs>() { Action = action, Args = args };
			queryable.Query.ForEach(ref idActionArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T>(this TQueryable queryable, TArgs args, IdActionRefArgs<T, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T, IdActionRefArgsAdapter<T, TArgs>>(ref idActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2>(this TQueryable queryable, TArgs args, IdActionRefArgs<T1, T2, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, IdActionRefArgsAdapter<T1, T2, TArgs>>(ref idActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3>(this TQueryable queryable, TArgs args, IdActionRefArgs<T1, T2, T3, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, IdActionRefArgsAdapter<T1, T2, T3, TArgs>>(ref idActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4>(this TQueryable queryable, TArgs args, IdActionRefArgs<T1, T2, T3, T4, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, T3, T4, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, IdActionRefArgsAdapter<T1, T2, T3, T4, TArgs>>(ref idActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4, T5>(this TQueryable queryable, TArgs args, IdActionRefArgs<T1, T2, T3, T4, T5, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, T3, T4, T5, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, IdActionRefArgsAdapter<T1, T2, T3, T4, T5, TArgs>>(ref idActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4, T5, T6>(this TQueryable queryable, TArgs args, IdActionRefArgs<T1, T2, T3, T4, T5, T6, TArgs> action)
			where TQueryable : IQueryable
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, T3, T4, T5, T6, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, T6, IdActionRefArgsAdapter<T1, T2, T3, T4, T5, T6, TArgs>>(ref idActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable>(this TQueryable queryable, EntityAction action)
			where TQueryable : IQueryable
		{
			var entityActionAdapter = new EntityActionAdapter { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World) };
			queryable.Query.ForEach(ref entityActionAdapter);
		}

		public static void ForEach<TQueryable, T>(this TQueryable queryable, EntityActionRef<T> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World) };
			queryable.Query.ForEach<T, EntityActionRefAdapter<T>>(ref entityActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2>(this TQueryable queryable, EntityActionRef<T1, T2> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World) };
			queryable.Query.ForEach<T1, T2, EntityActionRefAdapter<T1, T2>>(ref entityActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3>(this TQueryable queryable, EntityActionRef<T1, T2, T3> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World) };
			queryable.Query.ForEach<T1, T2, T3, EntityActionRefAdapter<T1, T2, T3>>(ref entityActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4>(this TQueryable queryable, EntityActionRef<T1, T2, T3, T4> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3, T4> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World) };
			queryable.Query.ForEach<T1, T2, T3, T4, EntityActionRefAdapter<T1, T2, T3, T4>>(ref entityActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4, T5>(this TQueryable queryable, EntityActionRef<T1, T2, T3, T4, T5> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3, T4, T5> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World) };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, EntityActionRefAdapter<T1, T2, T3, T4, T5>>(ref entityActionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4, T5, T6>(this TQueryable queryable, EntityActionRef<T1, T2, T3, T4, T5, T6> action)
			where TQueryable : IQueryable
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3, T4, T5, T6> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World) };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, T6, EntityActionRefAdapter<T1, T2, T3, T4, T5, T6>>(ref entityActionRefAdapter);
		}

		public static void ForEach<TQueryable, TArgs>(this TQueryable queryable, TArgs args, EntityActionArgs<TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionArgsAdapter = new EntityActionArgsAdapter<TArgs>() { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World), Args = args };
			queryable.Query.ForEach(ref entityActionArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T, TArgs> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World), Args = args };
			queryable.Query.ForEach<T, EntityActionRefArgsAdapter<T, TArgs>>(ref entityActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T1, T2, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World), Args = args };
			queryable.Query.ForEach<T1, T2, EntityActionRefArgsAdapter<T1, T2, TArgs>>(ref entityActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T1, T2, T3, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World), Args = args };
			queryable.Query.ForEach<T1, T2, T3, EntityActionRefArgsAdapter<T1, T2, T3, TArgs>>(ref entityActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T1, T2, T3, T4, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World), Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs>>(ref entityActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4, T5>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T1, T2, T3, T4, T5, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, T4, T5, TArgs> { Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World), Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, EntityActionRefArgsAdapter<T1, T2, T3, T4, T5, TArgs>>(ref entityActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4, T5, T6>(this TQueryable queryable, TArgs args, EntityActionRefArgs<T1, T2, T3, T4, T5, T6, TArgs> action)
			where TQueryable : IQueryable
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, T4, T5, T6, TArgs>
				{ Action = action, Entities = queryable.World.Entities, Entity = new Entity(Entifier.Dead, queryable.World), Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, T6, EntityActionRefArgsAdapter<T1, T2, T3, T4, T5, T6, TArgs>>(ref entityActionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, T>(this TQueryable queryable, ActionRef<T> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T> { Action = action };
			queryable.Query.ForEach<T, ActionRefAdapter<T>>(ref actionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2>(this TQueryable queryable, ActionRef<T1, T2> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2> { Action = action };
			queryable.Query.ForEach<T1, T2, ActionRefAdapter<T1, T2>>(ref actionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3>(this TQueryable queryable, ActionRef<T1, T2, T3> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, ActionRefAdapter<T1, T2, T3>>(ref actionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4>(this TQueryable queryable, ActionRef<T1, T2, T3, T4> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3, T4> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, T4, ActionRefAdapter<T1, T2, T3, T4>>(ref actionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4, T5>(this TQueryable queryable, ActionRef<T1, T2, T3, T4, T5> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3, T4, T5> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, ActionRefAdapter<T1, T2, T3, T4, T5>>(ref actionRefAdapter);
		}

		public static void ForEach<TQueryable, T1, T2, T3, T4, T5, T6>(this TQueryable queryable, ActionRef<T1, T2, T3, T4, T5, T6> action)
			where TQueryable : IQueryable
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3, T4, T5, T6> { Action = action };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, T6, ActionRefAdapter<T1, T2, T3, T4, T5, T6>>(ref actionRefAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T>(this TQueryable queryable, TArgs args, ActionRefArgs<T, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T, ActionRefArgsAdapter<T, TArgs>>(ref actionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2>(this TQueryable queryable, TArgs args, ActionRefArgs<T1, T2, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, ActionRefArgsAdapter<T1, T2, TArgs>>(ref actionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3>(this TQueryable queryable, TArgs args, ActionRefArgs<T1, T2, T3, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, ActionRefArgsAdapter<T1, T2, T3, TArgs>>(ref actionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4>(this TQueryable queryable, TArgs args, ActionRefArgs<T1, T2, T3, T4, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, T3, T4, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, ActionRefArgsAdapter<T1, T2, T3, T4, TArgs>>(ref actionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4, T5>(this TQueryable queryable, TArgs args, ActionRefArgs<T1, T2, T3, T4, T5, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, T3, T4, T5, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, ActionRefArgsAdapter<T1, T2, T3, T4, T5, TArgs>>(ref actionRefArgsAdapter);
		}

		public static void ForEach<TQueryable, TArgs, T1, T2, T3, T4, T5, T6>(this TQueryable queryable, TArgs args, ActionRefArgs<T1, T2, T3, T4, T5, T6, TArgs> action)
			where TQueryable : IQueryable
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, T3, T4, T5, T6, TArgs> { Action = action, Args = args };
			queryable.Query.ForEach<T1, T2, T3, T4, T5, T6, ActionRefArgsAdapter<T1, T2, T3, T4, T5, T6, TArgs>>(ref actionRefArgsAdapter);
		}

		public static void Fill<TQueryable>(this TQueryable queryable, IList<int> result)
			where TQueryable : IQueryable
		{
			var fillIds = new FillIds { Result = result };
			queryable.Query.ForEach(ref fillIds);
		}

		public static void Fill<TQueryable>(this TQueryable queryable, IList<Entifier> result)
			where TQueryable : IQueryable
		{
			var fillEntifiers = new FillEntifiers { Result = result, Entities = queryable.World.Entities };
			queryable.Query.ForEach(ref fillEntifiers);
		}

		public static int First<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			foreach (var id in queryable)
			{
				return id;
			}

			return Constants.InvalidId;
		}

		public static Entity FirstEntity<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			foreach (var id in queryable)
			{
				return queryable.World.GetEntity(id);
			}

			return new Entity(Entifier.Dead, queryable.World);
		}

		public static bool Any<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			foreach (var id in queryable)
			{
				return true;
			}

			return false;
		}

		public static void Destroy<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			var destroyEntities = new DestroyAll { Entities = queryable.World.Entities };
			queryable.Query.ForEach(ref destroyEntities);
		}

		public static int Count<TQueryable>(this TQueryable queryable)
			where TQueryable : IQueryable
		{
			var countEntities = new CountAll();
			queryable.Query.ForEach(ref countEntities);
			return countEntities.Result;
		}
	}
}

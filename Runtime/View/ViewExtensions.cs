using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public static class ViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView>(this TView view, EntityAction action)
			where TView : IView
		{
			var entityActionAdapter = new EntityActionAdapter { Action = action };
			view.ForEach(ref entityActionAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, EntityActionRef<T> action)
			where TView : IViewT
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T> { Action = action };
			view.ForEach<EntityActionRefAdapter<T>, T>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, EntityActionRef<T1, T2> action)
			where TView : IViewTT
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2> { Action = action };
			view.ForEach<EntityActionRefAdapter<T1, T2>, T1, T2>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, EntityActionRef<T1, T2, T3> action)
			where TView : IViewTTT
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3> { Action = action };
			view.ForEach<EntityActionRefAdapter<T1, T2, T3>, T1, T2, T3>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3, T4>(this TView view, EntityActionRef<T1, T2, T3, T4> action)
			where TView : IViewTTTT
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3, T4> { Action = action };
			view.ForEach<EntityActionRefAdapter<T1, T2, T3, T4>, T1, T2, T3, T4>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs>(this TView view, TArgs args, EntityActionArgs<TArgs> action)
			where TView : IView
		{
			var entityActionArgsAdapter = new EntityActionArgsAdapter<TArgs>() { Action = action, Args = args };
			view.ForEach(ref entityActionArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T>(this TView view, TArgs args, EntityActionRefArgs<T, TArgs> action)
			where TView : IViewT
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T, TArgs> { Action = action, Args = args };
			view.ForEach<EntityActionRefArgsAdapter<T, TArgs>, T>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2>(this TView view, TArgs args, EntityActionRefArgs<T1, T2, TArgs> action)
			where TView : IViewTT
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Args = args };
			view.ForEach<EntityActionRefArgsAdapter<T1, T2, TArgs>, T1, T2>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2, T3>(this TView view, TArgs args, EntityActionRefArgs<T1, T2, T3, TArgs> action)
			where TView : IViewTTT
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Args = args };
			view.ForEach<EntityActionRefArgsAdapter<T1, T2, T3, TArgs>, T1, T2, T3>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2, T3, T4>(this TView view, TArgs args, EntityActionRefArgs<T1, T2, T3, T4, TArgs> action)
			where TView : IViewTTTT
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs> { Action = action, Args = args };
			view.ForEach<EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs>, T1, T2, T3, T4>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, ActionRef<T> action)
			where TView : IViewT
		{
			var actionRefAdapter = new ActionRefAdapter<T> { Action = action };
			view.ForEach<ActionRefAdapter<T>, T>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, ActionRef<T1, T2> action)
			where TView : IViewTT
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2> { Action = action };
			view.ForEach<ActionRefAdapter<T1, T2>, T1, T2>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, ActionRef<T1, T2, T3> action)
			where TView : IViewTTT
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3> { Action = action };
			view.ForEach<ActionRefAdapter<T1, T2, T3>, T1, T2, T3>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3, T4>(this TView view, ActionRef<T1, T2, T3, T4> action)
			where TView : IViewTTTT
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3, T4> { Action = action };
			view.ForEach<ActionRefAdapter<T1, T2, T3, T4>, T1, T2, T3, T4>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T>(this TView view, TArgs args, ActionRefArgs<T, TArgs> action)
			where TView : IViewT
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T, TArgs> { Action = action, Args = args };
			view.ForEach<ActionRefArgsAdapter<T, TArgs>, T>(ref actionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2>(this TView view, TArgs args, ActionRefArgs<T1, T2, TArgs> action)
			where TView : IViewTT
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Args = args };
			view.ForEach<ActionRefArgsAdapter<T1, T2, TArgs>, T1, T2>(ref actionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2, T3>(this TView view, TArgs args, ActionRefArgs<T1, T2, T3, TArgs> action)
			where TView : IViewTTT
		{
			var actionRefArgsAdapter = new ActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Args = args };
			view.ForEach<ActionRefArgsAdapter<T1, T2, T3, TArgs>, T1, T2, T3>(ref actionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TView>(this TView view, IList<int> result)
			where TView : IView
		{
			var fillIds = new FillIds { Result = result };
			view.ForEach(ref fillIds);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TView>(this TView view, IList<Entity> result)
			where TView : IView
		{
			var fillEntities = new FillEntities { Result = result, Entities = view.World.Entities };
			view.ForEach(ref fillEntities);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int First<TView>(this TView view)
			where TView : IView
		{
			var returnFirst = new ReturnFirst() { Result = Constants.InvalidId };
			view.ForEach(ref returnFirst);
			return returnFirst.Result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity FirstEntity<TView>(this TView view)
			where TView : IView
		{
			var returnFirstEntity = new ReturnFirst { Result = Constants.InvalidId };
			view.ForEach(ref returnFirstEntity);
			return returnFirstEntity.Result == Constants.InvalidId
				? Entity.Dead
				: view.World.GetEntity(returnFirstEntity.Result);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy<TView>(this TView view)
			where TView : IView
		{
			var destroyEntities = new DestroyAll { Entities = view.World.Entities };
			view.ForEach(ref destroyEntities);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int Count<TView>(this TView view)
			where TView : IView
		{
			var countEntities = new CountAll();
			view.ForEach(ref countEntities);
			return countEntities.Result;
		}
	}
}

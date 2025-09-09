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
		public static void ForEach<TView>(this TView view, IdAction action)
			where TView : IView
		{
			var idActionAdapter = new IdActionAdapter { Action = action };
			view.ForEach(ref idActionAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, IdActionRef<T> action)
			where TView : IViewT
		{
			var idActionRefAdapter = new IdActionRefAdapter<T> { Action = action };
			view.ForEach<IdActionRefAdapter<T>, T>(ref idActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, IdActionRef<T1, T2> action)
			where TView : IViewTT
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2> { Action = action };
			view.ForEach<IdActionRefAdapter<T1, T2>, T1, T2>(ref idActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, IdActionRef<T1, T2, T3> action)
			where TView : IViewTTT
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2, T3> { Action = action };
			view.ForEach<IdActionRefAdapter<T1, T2, T3>, T1, T2, T3>(ref idActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3, T4>(this TView view, IdActionRef<T1, T2, T3, T4> action)
			where TView : IViewTTTT
		{
			var idActionRefAdapter = new IdActionRefAdapter<T1, T2, T3, T4> { Action = action };
			view.ForEach<IdActionRefAdapter<T1, T2, T3, T4>, T1, T2, T3, T4>(ref idActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs>(this TView view, TArgs args, IdActionArgs<TArgs> action)
			where TView : IView
		{
			var idActionArgsAdapter = new IdActionArgsAdapter<TArgs>() { Action = action, Args = args };
			view.ForEach(ref idActionArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T>(this TView view, TArgs args, IdActionRefArgs<T, TArgs> action)
			where TView : IViewT
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T, TArgs> { Action = action, Args = args };
			view.ForEach<IdActionRefArgsAdapter<T, TArgs>, T>(ref idActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2>(this TView view, TArgs args, IdActionRefArgs<T1, T2, TArgs> action)
			where TView : IViewTT
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Args = args };
			view.ForEach<IdActionRefArgsAdapter<T1, T2, TArgs>, T1, T2>(ref idActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2, T3>(this TView view, TArgs args, IdActionRefArgs<T1, T2, T3, TArgs> action)
			where TView : IViewTTT
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Args = args };
			view.ForEach<IdActionRefArgsAdapter<T1, T2, T3, TArgs>, T1, T2, T3>(ref idActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2, T3, T4>(this TView view, TArgs args, IdActionRefArgs<T1, T2, T3, T4, TArgs> action)
			where TView : IViewTTTT
		{
			var idActionRefArgsAdapter = new IdActionRefArgsAdapter<T1, T2, T3, T4, TArgs> { Action = action, Args = args };
			view.ForEach<IdActionRefArgsAdapter<T1, T2, T3, T4, TArgs>, T1, T2, T3, T4>(ref idActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView>(this TView view, EntityAction action)
			where TView : IView
		{
			var entityActionAdapter = new EntityActionAdapter { Action = action, Entifiers = view.World.Entifiers, World = view.World };
			view.ForEach(ref entityActionAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, EntityActionRef<T> action)
			where TView : IViewT
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T> { Action = action, Entifiers = view.World.Entifiers, World = view.World };
			view.ForEach<EntityActionRefAdapter<T>, T>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, EntityActionRef<T1, T2> action)
			where TView : IViewTT
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2> { Action = action, Entifiers = view.World.Entifiers, World = view.World };
			view.ForEach<EntityActionRefAdapter<T1, T2>, T1, T2>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, EntityActionRef<T1, T2, T3> action)
			where TView : IViewTTT
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3> { Action = action, Entifiers = view.World.Entifiers, World = view.World };
			view.ForEach<EntityActionRefAdapter<T1, T2, T3>, T1, T2, T3>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3, T4>(this TView view, EntityActionRef<T1, T2, T3, T4> action)
			where TView : IViewTTTT
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3, T4> { Action = action, Entifiers = view.World.Entifiers, World = view.World };
			view.ForEach<EntityActionRefAdapter<T1, T2, T3, T4>, T1, T2, T3, T4>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs>(this TView view, TArgs args, EntityActionArgs<TArgs> action)
			where TView : IView
		{
			var entityActionArgsAdapter = new EntityActionArgsAdapter<TArgs>() { Action = action, Entifiers = view.World.Entifiers, World = view.World, Args = args };
			view.ForEach(ref entityActionArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T>(this TView view, TArgs args, EntityActionRefArgs<T, TArgs> action)
			where TView : IViewT
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T, TArgs> { Action = action, Entifiers = view.World.Entifiers, World = view.World, Args = args };
			view.ForEach<EntityActionRefArgsAdapter<T, TArgs>, T>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2>(this TView view, TArgs args, EntityActionRefArgs<T1, T2, TArgs> action)
			where TView : IViewTT
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, TArgs> { Action = action, Entifiers = view.World.Entifiers, World = view.World, Args = args };
			view.ForEach<EntityActionRefArgsAdapter<T1, T2, TArgs>, T1, T2>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2, T3>(this TView view, TArgs args, EntityActionRefArgs<T1, T2, T3, TArgs> action)
			where TView : IViewTTT
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, TArgs> { Action = action, Entifiers = view.World.Entifiers, World = view.World, Args = args };
			view.ForEach<EntityActionRefArgsAdapter<T1, T2, T3, TArgs>, T1, T2, T3>(ref entityActionRefArgsAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, TArgs, T1, T2, T3, T4>(this TView view, TArgs args, EntityActionRefArgs<T1, T2, T3, T4, TArgs> action)
			where TView : IViewTTTT
		{
			var entityActionRefArgsAdapter = new EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs> { Action = action, Entifiers = view.World.Entifiers, World = view.World, Args = args };
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
		public static void Fill<TView>(this TView view, IList<Entifier> result)
			where TView : IView
		{
			var fillEntifiers = new FillEntifiers { Result = result, Entifiers = view.World.Entifiers };
			view.ForEach(ref fillEntifiers);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int First<TView>(this TView view)
			where TView : IView
		{
			foreach (var id in view)
			{
				return id;
			}

			return Constants.InvalidId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Entity FirstEntity<TView>(this TView view)
			where TView : IView
		{
			foreach (var id in view)
			{
				return view.World.GetEntity(id);
			}

			return new Entity(Entifier.Dead, view.World);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy<TView>(this TView view)
			where TView : IView
		{
			var destroyEntities = new DestroyAll { Entifiers = view.World.Entifiers };
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

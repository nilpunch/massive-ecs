using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
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
			where TView : IView
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T> { Action = action };
			view.ForEach<EntityActionRefAdapter<T>, T>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, EntityActionRef<T1, T2> action)
			where TView : IView
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2> { Action = action };
			view.ForEach<EntityActionRefAdapter<T1, T2>, T1, T2>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, EntityActionRef<T1, T2, T3> action)
			where TView : IView
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3> { Action = action };
			view.ForEach<EntityActionRefAdapter<T1, T2, T3>, T1, T2, T3>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3, T4>(this TView view, EntityActionRef<T1, T2, T3, T4> action)
			where TView : IView
		{
			var entityActionRefAdapter = new EntityActionRefAdapter<T1, T2, T3, T4> { Action = action };
			view.ForEach<EntityActionRefAdapter<T1, T2, T3, T4>, T1, T2, T3, T4>(ref entityActionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra>(this TView view, TExtra extra, EntityActionExtra<TExtra> action)
			where TView : IView
		{
			var entityActionExtraAdapter = new EntityActionExtraAdapter<TExtra>() { Action = action, Extra = extra };
			view.ForEach(ref entityActionExtraAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T>(this TView view, TExtra extra, EntityActionRefExtra<T, TExtra> action)
			where TView : IView
		{
			var entityActionRefExtraAdapter = new EntityActionRefExtraAdapter<T, TExtra> { Action = action, Extra = extra };
			view.ForEach<EntityActionRefExtraAdapter<T, TExtra>, T>(ref entityActionRefExtraAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2>(this TView view, TExtra extra, EntityActionRefExtra<T1, T2, TExtra> action)
			where TView : IView
		{
			var entityActionRefExtraAdapter = new EntityActionRefExtraAdapter<T1, T2, TExtra> { Action = action, Extra = extra };
			view.ForEach<EntityActionRefExtraAdapter<T1, T2, TExtra>, T1, T2>(ref entityActionRefExtraAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2, T3>(this TView view, TExtra extra, EntityActionRefExtra<T1, T2, T3, TExtra> action)
			where TView : IView
		{
			var entityActionRefExtraAdapter = new EntityActionRefExtraAdapter<T1, T2, T3, TExtra> { Action = action, Extra = extra };
			view.ForEach<EntityActionRefExtraAdapter<T1, T2, T3, TExtra>, T1, T2, T3>(ref entityActionRefExtraAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2, T3, T4>(this TView view, TExtra extra, EntityActionRefExtra<T1, T2, T3, T4, TExtra> action)
			where TView : IView
		{
			var entityActionRefExtraAdapter = new EntityActionRefExtraAdapter<T1, T2, T3, T4, TExtra> { Action = action, Extra = extra };
			view.ForEach<EntityActionRefExtraAdapter<T1, T2, T3, T4, TExtra>, T1, T2, T3, T4>(ref entityActionRefExtraAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, ActionRef<T> action)
			where TView : IView
		{
			var actionRefAdapter = new ActionRefAdapter<T> { Action = action };
			view.ForEach<ActionRefAdapter<T>, T>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, ActionRef<T1, T2> action)
			where TView : IView
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2> { Action = action };
			view.ForEach<ActionRefAdapter<T1, T2>, T1, T2>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, ActionRef<T1, T2, T3> action)
			where TView : IView
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3> { Action = action };
			view.ForEach<ActionRefAdapter<T1, T2, T3>, T1, T2, T3>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3, T4>(this TView view, ActionRef<T1, T2, T3, T4> action)
			where TView : IView
		{
			var actionRefAdapter = new ActionRefAdapter<T1, T2, T3, T4> { Action = action };
			view.ForEach<ActionRefAdapter<T1, T2, T3, T4>, T1, T2, T3, T4>(ref actionRefAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T>(this TView view, TExtra extra, ActionRefExtra<T, TExtra> action)
			where TView : IView
		{
			var actionRefExtraAdapter = new ActionRefExtraAdapter<T, TExtra> { Action = action, Extra = extra };
			view.ForEach<ActionRefExtraAdapter<T, TExtra>, T>(ref actionRefExtraAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2>(this TView view, TExtra extra, ActionRefExtra<T1, T2, TExtra> action)
			where TView : IView
		{
			var actionRefExtraAdapter = new ActionRefExtraAdapter<T1, T2, TExtra> { Action = action, Extra = extra };
			view.ForEach<ActionRefExtraAdapter<T1, T2, TExtra>, T1, T2>(ref actionRefExtraAdapter);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2, T3>(this TView view, TExtra extra, ActionRefExtra<T1, T2, T3, TExtra> action)
			where TView : IView
		{
			var actionRefExtraAdapter = new ActionRefExtraAdapter<T1, T2, T3, TExtra> { Action = action, Extra = extra };
			view.ForEach<ActionRefExtraAdapter<T1, T2, T3, TExtra>, T1, T2, T3>(ref actionRefExtraAdapter);
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
			var fillEntities = new FillEntities { Result = result, Entities = view.Registry.Entities };
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
			var returnFirstEntity = new ReturnFirstEntity { Result = Entity.Dead, Entities = view.Registry.Entities };
			view.ForEach(ref returnFirstEntity);
			return returnFirstEntity.Result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Destroy<TView>(this TView view)
			where TView : IView
		{
			var destroyEntities = new DestroyAll { Entities = view.Registry.Entities };
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

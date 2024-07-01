using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class ViewExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView>(this TView view, EntityAction action)
			where TView : IView
		{
			view.ForEach(new EntityActionAdapter { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, EntityActionRef<T> action)
			where TView : IView
		{
			view.ForEach<EntityActionRefAdapter<T>, T>(new EntityActionRefAdapter<T> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, EntityActionRef<T1, T2> action)
			where TView : IView
		{
			view.ForEach<EntityActionRefAdapter<T1, T2>, T1, T2>(new EntityActionRefAdapter<T1, T2> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, EntityActionRef<T1, T2, T3> action)
			where TView : IView
		{
			view.ForEach<EntityActionRefAdapter<T1, T2, T3>, T1, T2, T3>(new EntityActionRefAdapter<T1, T2, T3> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra>(this TView view, TExtra extra, EntityActionExtra<TExtra> action)
			where TView : IView
		{
			view.ForEach(new EntityActionExtraAdapter<TExtra>() { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T>(this TView view, TExtra extra, EntityActionRefExtra<T, TExtra> action)
			where TView : IView
		{
			view.ForEach<EntityActionRefExtraAdapter<T, TExtra>, T>(new EntityActionRefExtraAdapter<T, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2>(this TView view, TExtra extra, EntityActionRefExtra<T1, T2, TExtra> action)
			where TView : IView
		{
			view.ForEach<EntityActionRefExtraAdapter<T1, T2, TExtra>, T1, T2>(new EntityActionRefExtraAdapter<T1, T2, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2, T3>(this TView view, TExtra extra, EntityActionRefExtra<T1, T2, T3, TExtra> action)
			where TView : IView
		{
			view.ForEach<EntityActionRefExtraAdapter<T1, T2, T3, TExtra>, T1, T2, T3>(new EntityActionRefExtraAdapter<T1, T2, T3, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, ActionRef<T> action)
			where TView : IView
		{
			view.ForEach<ActionRefAdapter<T>, T>(new ActionRefAdapter<T> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, ActionRef<T1, T2> action)
			where TView : IView
		{
			view.ForEach<ActionRefAdapter<T1, T2>, T1, T2>(new ActionRefAdapter<T1, T2> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, ActionRef<T1, T2, T3> action)
			where TView : IView
		{
			view.ForEach<ActionRefAdapter<T1, T2, T3>, T1, T2, T3>(new ActionRefAdapter<T1, T2, T3> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T>(this TView view, TExtra extra, ActionRefExtra<T, TExtra> action)
			where TView : IView
		{
			view.ForEach<ActionRefExtraAdapter<T, TExtra>, T>(new ActionRefExtraAdapter<T, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2>(this TView view, TExtra extra, ActionRefExtra<T1, T2, TExtra> action)
			where TView : IView
		{
			view.ForEach<ActionRefExtraAdapter<T1, T2, TExtra>, T1, T2>(new ActionRefExtraAdapter<T1, T2, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2, T3>(this TView view, TExtra extra, ActionRefExtra<T1, T2, T3, TExtra> action)
			where TView : IView
		{
			view.ForEach<ActionRefExtraAdapter<T1, T2, T3, TExtra>, T1, T2, T3>(new ActionRefExtraAdapter<T1, T2, T3, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TView>(this TView view, IList<int> result)
			where TView : IView
		{
			view.ForEach(new FillIds { Result = result });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TView>(this TView view, IList<Entity> result)
			where TView : IView
		{
			view.ForEach(new FillEntities { Result = result, Entities = view.Registry.Entities });
		}
	}
}

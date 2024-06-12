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
			view.ForEachUniversal(new EntityActionInvoker { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, EntityActionRef<T> action)
			where TView : IView<T>
		{
			view.ForEachUniversal(new EntityActionRefInvoker<T> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, EntityActionRef<T1, T2> action)
			where TView : IView<T1, T2>
		{
			view.ForEachUniversal(new EntityActionRefInvoker<T1, T2> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, EntityActionRef<T1, T2, T3> action)
			where TView : IView<T1, T2, T3>
		{
			view.ForEachUniversal(new EntityActionRefInvoker<T1, T2, T3> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra>(this TView view, TExtra extra, EntityActionExtra<TExtra> action)
			where TView : IView
		{
			view.ForEachUniversal(new EntityActionExtraInvoker<TExtra>() { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T>(this TView view, TExtra extra, EntityActionRefExtra<T, TExtra> action)
			where TView : IView<T>
		{
			view.ForEachUniversal(new EntityActionRefExtraInvoker<T, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2>(this TView view, TExtra extra, EntityActionRefExtra<T1, T2, TExtra> action)
			where TView : IView<T1, T2>
		{
			view.ForEachUniversal(new EntityActionRefExtraInvoker<T1, T2, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2, T3>(this TView view, TExtra extra, EntityActionRefExtra<T1, T2, T3, TExtra> action)
			where TView : IView<T1, T2, T3>
		{
			view.ForEachUniversal(new EntityActionRefExtraInvoker<T1, T2, T3, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T>(this TView view, ActionRef<T> action)
			where TView : IView<T>
		{
			view.ForEachUniversal(new ActionRefInvoker<T> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2>(this TView view, ActionRef<T1, T2> action)
			where TView : IView<T1, T2>
		{
			view.ForEachUniversal(new ActionRefInvoker<T1, T2> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEach<TView, T1, T2, T3>(this TView view, ActionRef<T1, T2, T3> action)
			where TView : IView<T1, T2, T3>
		{
			view.ForEachUniversal(new ActionRefInvoker<T1, T2, T3> { Action = action });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T>(this TView view, TExtra extra, ActionRefExtra<T, TExtra> action)
			where TView : IView<T>
		{
			view.ForEachUniversal(new ActionRefExtraInvoker<T, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2>(this TView view, TExtra extra, ActionRefExtra<T1, T2, TExtra> action)
			where TView : IView<T1, T2>
		{
			view.ForEachUniversal(new ActionRefExtraInvoker<T1, T2, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ForEachExtra<TView, TExtra, T1, T2, T3>(this TView view, TExtra extra, ActionRefExtra<T1, T2, T3, TExtra> action)
			where TView : IView<T1, T2, T3>
		{
			view.ForEachUniversal(new ActionRefExtraInvoker<T1, T2, T3, TExtra> { Action = action, Extra = extra });
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Fill<TView>(this TView view, IList<int> result)
			where TView : IView
		{
			view.ForEachUniversal(new EntityFillInvoker { Result = result });
		}
	}
}

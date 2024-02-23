namespace Massive.Samples.ECS
{
	public static class ViewExtensions
	{
		public static void ForEach<TView, T>(this TView view, EntityAction action) where TView : IView<T> where T : struct
		{
			view.ForEach((int id, ref T _) => action.Invoke(id));
		}

		public static void ForEach<TView, T>(this TView view, ActionRef<T> action) where TView : IView<T> where T : struct
		{
			view.ForEach((int _, ref T value) => action.Invoke(ref value));
		}

		public static void ForEach<TView, T1, T2>(this TView view, EntityAction action) where TView : IView<T1, T2> where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 _, ref T2 _) => action.Invoke(id));
		}

		public static void ForEach<TView, T1, T2>(this TView view, ActionRef<T1> action) where TView : IView<T1, T2> where T1 : struct where T2 : struct
		{
			view.ForEach((int _, ref T1 value1, ref T2 _) => action.Invoke(ref value1));
		}

		public static void ForEach<TView, T1, T2>(this TView view, ActionRef<T2> action) where TView : IView<T1, T2> where T1 : struct where T2 : struct
		{
			view.ForEach((int _, ref T1 _, ref T2 value2) => action.Invoke(ref value2));
		}

		public static void ForEach<TView, T1, T2>(this TView view, EntityActionRef<T1> action) where TView : IView<T1, T2> where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 value1, ref T2 _) => action.Invoke(id, ref value1));
		}

		public static void ForEach<TView, T1, T2>(this TView view, EntityActionRef<T2> action) where TView : IView<T1, T2> where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 _, ref T2 value2) => action.Invoke(id, ref value2));
		}

		public static void ForEach<TView, T1, T2>(this TView view, ActionRef<T1, T2> action) where TView : IView<T1, T2> where T1 : struct where T2 : struct
		{
			view.ForEach((int _, ref T1 value1, ref T2 value2) => action.Invoke(ref value1, ref value2));
		}
	}
}
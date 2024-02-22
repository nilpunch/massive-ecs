namespace Massive.Samples.ECS
{
	public static class ViewExtensions
	{
		public static void ForEach<T>(this View<T> view, EntityAction action) where T : struct
		{
			view.ForEach((int id, ref T value) => action.Invoke(id));
		}

		public static void ForEach<T>(this View<T> view, ActionRef<T> action) where T : struct
		{
			view.ForEach((int id, ref T value) => action.Invoke(ref value));
		}

		public static void ForEach<T1, T2>(this View<T1, T2> view, EntityAction action) where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(id));
		}

		public static void ForEach<T1, T2>(this View<T1, T2> view, ActionRef<T1> action) where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(ref value1));
		}

		public static void ForEach<T1, T2>(this View<T1, T2> view, ActionRef<T2> action) where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(ref value2));
		}

		public static void ForEach<T1, T2>(this View<T1, T2> view, EntityActionRef<T1> action) where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(id, ref value1));
		}

		public static void ForEach<T1, T2>(this View<T1, T2> view, EntityActionRef<T2> action) where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(id, ref value2));
		}

		public static void ForEach<T1, T2>(this View<T1, T2> view, ActionRef<T1, T2> action) where T1 : struct where T2 : struct
		{
			view.ForEach((int id, ref T1 value1, ref T2 value2) => action.Invoke(ref value1, ref value2));
		}
	}
}
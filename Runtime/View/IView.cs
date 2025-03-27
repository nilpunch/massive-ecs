namespace Massive
{
	public interface IView
	{
		World World { get; }

		void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction;
	}

	public interface IViewT : IView
	{
		void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>;
	}

	public interface IViewTT : IView
	{
		void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>;
	}

	public interface IViewTTT : IView
	{
		void ForEach<TAction, T1, T2, T3>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>;
	}

	public interface IViewTTTT : IView
	{
		void ForEach<TAction, T1, T2, T3, T4>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>;
	}
}

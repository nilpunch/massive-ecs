namespace Massive
{
	public interface IView
	{
		Registry Registry { get; }

		void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction;

		void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>;

		void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>;

		void ForEach<TAction, T1, T2, T3>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>;

		void ForEach<TAction, T1, T2, T3, T4>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>;
	}
}

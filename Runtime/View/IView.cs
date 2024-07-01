namespace Massive
{
	public interface IView
	{
		IRegistry Registry { get; }

		void ForEach<TAction>(TAction action)
			where TAction : IEntityAction;

		void ForEach<TAction, T>(TAction action)
			where TAction : IEntityAction<T>;

		void ForEach<TAction, T1, T2>(TAction action)
			where TAction : IEntityAction<T1, T2>;

		void ForEach<TAction, T1, T2, T3>(TAction action)
			where TAction : IEntityAction<T1, T2, T3>;
	}
}

namespace Massive
{
	public interface IView
	{
		IRegistry Registry { get; }

		void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker;

		void ForEachUniversal<TInvoker, T>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T>;

		void ForEachUniversal<TInvoker, T1, T2>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2>;

		void ForEachUniversal<TInvoker, T1, T2, T3>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2, T3>;
	}
}

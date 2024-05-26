namespace Massive
{
	public interface IView<T1, T2, T3>
	{
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T1, T2, T3>;
	}
}

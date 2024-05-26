namespace Massive
{
	public interface IView<T>
	{
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T>;
	}
}

namespace Massive
{
	public interface IView
	{
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker;
	}
}

namespace Massive
{
	public interface IInject<TArg>
	{
		void Inject(TArg arg);
	}
}

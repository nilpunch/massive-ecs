namespace Massive
{
	public interface ISystemInject<TArg>
	{
		void Inject(TArg arg);
	}
}

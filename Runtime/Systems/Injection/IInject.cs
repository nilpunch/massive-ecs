namespace Massive
{
	public interface IInject<in TArg>
	{
		void Inject(TArg arg);
	}
}

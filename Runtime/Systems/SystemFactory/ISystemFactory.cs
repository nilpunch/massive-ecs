namespace Massive
{
	public interface ISystemFactory
	{
		ISystem Create();

		int Order => 0;
	}
}

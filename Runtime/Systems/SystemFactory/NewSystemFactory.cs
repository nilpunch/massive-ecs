namespace Massive
{
	public class NewSystemFactory<T> : ISystemFactory where T : ISystem, new()
	{
		public int Order { get; }

		public NewSystemFactory(int order = 0)
		{
			Order = order;
		}

		public ISystem Create()
		{
			return new T();
		}
	}
}

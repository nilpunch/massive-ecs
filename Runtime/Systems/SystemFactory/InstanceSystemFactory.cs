namespace Massive
{
	public class InstanceSystemFactory : ISystemFactory
	{
		private readonly ISystem _system;

		public int Order { get; }

		public InstanceSystemFactory(ISystem system, int order = 0)
		{
			_system = system;
			Order = order;
		}

		public ISystem Create()
		{
			return _system;
		}
	}
}

namespace Massive
{
	public class InstanceSystemFactory : ISystemFactory
	{
		private readonly ISystem _system;

		public InstanceSystemFactory(ISystem system)
		{
			_system = system;
		}

		public ISystem Create()
		{
			return _system;
		}
	}
}

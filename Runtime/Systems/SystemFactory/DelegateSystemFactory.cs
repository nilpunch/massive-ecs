using System;

namespace Massive
{
	public class DelegateSystemFactory : ISystemFactory
	{
		private readonly int FactoryTypeHash = nameof(DelegateSystemFactory).GetHashCode();

		private readonly Func<ISystem> _factory;

		public int Order { get; }

		public DelegateSystemFactory(Func<ISystem> factory, int order = 0)
		{
			_factory = factory;
			Order = order;
		}

		public ISystem Create()
		{
			return _factory.Invoke();
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Order, FactoryTypeHash);
		}
	}
}

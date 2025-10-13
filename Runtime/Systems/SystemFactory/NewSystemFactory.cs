using System;

namespace Massive
{
	public class NewSystemFactory<T> : ISystemFactory where T : ISystem, new()
	{
		private readonly int FactoryTypeHash = nameof(NewSystemFactory<T>).GetHashCode();

		public int Order { get; }

		public NewSystemFactory(int order = 0)
		{
			Order = order;
		}

		public ISystem Create()
		{
			return new T();
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Order, FactoryTypeHash);
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public partial class Systems
	{
		private readonly List<ISystemFactory> _registeredFactories = new List<ISystemFactory>();

		public Allocator Allocator { get; } = new Allocator();

		public Systems Build()
		{
			Allocator.Reset();
			_systems = new ISystem[_registeredFactories.Count];
			var systemsCount = 0;
			foreach (var systemFactory in _registeredFactories.OrderBy(factory => factory.Order))
			{
				var system = systemFactory.Create();
				system.Build(systemsCount, Allocator);
				_systems[systemsCount++] = system;
			}
			Array.Clear(_systemsLookup, 0, _systemsLookup.Length);
			foreach (var system in _systems)
			{
				if (system is ISystemsCallback callback)
				{
					callback.AfterBuilded(this);
				}
			}
			return this;
		}

		public Systems Clear()
		{
			_registeredFactories.Clear();
			return this;
		}

		public Systems New(Func<ISystem> factory)
		{
			return Factory(new DelegateSystemFactory(factory));
		}

		public Systems New<T>() where T : ISystem, new()
		{
			return Factory(new NewSystemFactory<T>());
		}

		public Systems Instance(ISystem system)
		{
			return Factory(new InstanceSystemFactory(system));
		}

		public Systems Factory(ISystemFactory systemFactory)
		{
			_registeredFactories.Add(systemFactory);
			return this;
		}
	}
}

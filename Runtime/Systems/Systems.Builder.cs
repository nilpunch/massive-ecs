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

		public Systems New(Func<ISystem> factory, int order = 0)
		{
			return Factory(new DelegateSystemFactory(factory, order));
		}

		public Systems New<T>(int order = 0) where T : ISystem, new()
		{
			return Factory(new NewSystemFactory<T>(order));
		}

		public Systems Instance(ISystem system, int order = 0)
		{
			return Factory(new InstanceSystemFactory(system, order));
		}

		public Systems Factory(ISystemFactory systemFactory)
		{
			_registeredFactories.Add(systemFactory);
			return this;
		}
	}
}

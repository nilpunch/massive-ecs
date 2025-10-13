using System;
using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public partial class Systems
	{
		private readonly List<ISystemFactory> _registeredFactories = new List<ISystemFactory>();

		public Systems Build(World world)
		{
			_systems = new ISystem[_registeredFactories.Count];
			var systemsCount = 0;
			foreach (var systemFactory in _registeredFactories.OrderBy(factory => factory.Order))
			{
				var system = systemFactory.Create();
				system.World = world;
				system.Order = systemsCount;
				_systems[systemsCount++] = system;
			}
			_systemsCache.Clear();
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

		/// <summary>
		/// For immutable systems only.<br/>
		/// System instances will be shared across worlds when copying or cloning.<br/>
		/// </summary>
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

using System.Collections.Generic;
using System.Linq;

namespace Massive
{
	public class SystemsBuilder
	{
		private readonly List<ISystemFactory> _systemFactories = new List<ISystemFactory>();

		public SystemsBuilder Factory(ISystemFactory systemFactory)
		{
			_systemFactories.Add(systemFactory);
			return this;
		}

		public SystemsBuilder New<T>() where T : ISystem, new()
		{
			_systemFactories.Add(new NewSystemFactory<T>());
			return this;
		}

		public SystemsBuilder Instance(ISystem system)
		{
			_systemFactories.Add(new InstanceSystemFactory(system));
			return this;
		}

		public Systems Build(World world)
		{
			var systems = new Systems(world);
			foreach (var systemFactory in _systemFactories.OrderBy(factory => factory.Order))
			{
				systems.AddSystem(systemFactory.Create());
			}
			return systems;
		}
	}
}

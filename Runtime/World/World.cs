﻿using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class World
	{
		public Entities Entities { get; }

		public SetRegistry SetRegistry { get; }
		public FilterRegistry FilterRegistry { get; }

		public WorldConfig Config { get; }

		public World()
			: this(new WorldConfig())
		{
		}

		public World(WorldConfig worldConfig)
		{
			Entities = new Entities();
			SetRegistry = new SetRegistry(new SetFactory(worldConfig));
			FilterRegistry = new FilterRegistry(SetRegistry);
			Config = worldConfig;

			var allSets = SetRegistry.AllSets;
			Entities.BeforeDestroyed += RemoveFromAllSets;

			void RemoveFromAllSets(int entityId)
			{
				var setCount = allSets.Count;
				var sets = allSets.Items;
				for (var i = setCount - 1; i >= 0; i--)
				{
					sets[i].Remove(entityId);
				}
			}
		}
	}
}

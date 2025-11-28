namespace Massive.Samples.AllocatorUsage
{
	public struct InventoryFast : IAutoFree<InventoryFast>
	{
		public ListModel<Entifier> Items;
	}

	class WorldAllocatorSampleMaxPerformance
	{
		World World = new World();

		void Allocate()
		{
			var entity = World.Create();

			World.Set(entity, new InventoryFast()
			{
				Items = World.AllocListModel<Entifier>()
			});
		}

		void Use()
		{
			var entity = World.Include<InventoryFast>().First();

			ref var inventory = ref World.Get<InventoryFast>(entity);

			// Don't forget to use ref to work with the actual storage reference, not a copy.
			ref var items = ref inventory.Items;

			items.Add(World, World.CreateEntifier<Item>());
			items.Add(World, World.CreateEntifier<Item>());
			items.Add(World, World.CreateEntifier<Item>());
			items.Add(World, World.CreateEntifier<Item>());

			items.RemoveAt(World, 2);

			items.RemoveAtSwapBack(World, 0);

			foreach (var item in items.GetEnumerator(World))
			{
			}
		}

		void Free()
		{
			World.Clear();
		}
	}
}

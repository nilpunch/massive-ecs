namespace Massive.Samples.Basic
{
	public struct Item { }

	public struct Inventory
	{
		public ListHandle<Entity> Items;
	}

	class WorldAllocatorSample
	{
		World World = new World();

		void Allocate()
		{
			var entity = World.Create();

			// Allocates a list that's tied to this entity's lifetime.
			// It will be freed automatically when the entity is destroyed.
			var items = World.AllocAutoList<Entity>(entity);

			// Assign the list handle to the Inventory component.
			World.Set(entity, new Inventory()
			{
				Items = items
			});
		}

		void Use()
		{
			// Get the first entity that has an Inventory.
			var entity = World.View().Include<Inventory>().First();

			// Get a reference to its Inventory component.
			ref var inventory = ref World.Get<Inventory>(entity);

			// To access the items list, combine the list handle with the world.
			var items = inventory.Items.In(World);

			// You can treat it like a regular list.
			// It will resize automatically.
			items.Add(World.CreateEntity<Item>());
			items.Add(World.CreateEntity<Item>());
			items.Add(World.CreateEntity<Item>());
			items.Add(World.CreateEntity<Item>());

			// Remove an item by index.
			items.RemoveAt(2);

			// Fast removal without preserving order.
			items.RemoveAtSwapBack(0);

			// Iterate through the remaining items.
			foreach (var item in items)
			{
			}
		}

		void Free()
		{
			// This clears the world and destroys all entities.
			// All auto-allocated lists will be freed with their entities.
			World.Clear();
		}
	}
}

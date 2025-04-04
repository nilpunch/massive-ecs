namespace Massive.Samples.Basic
{
	class WorldUsageSample
	{
		static void Main()
		{
			var world = new World();

			var entity1 = world.CreateEntity(); // Creates unique entity.

			var entity2 = world.CreateEntity<int>(); // Creates unique entity with a component.

			world.Add<int>(entity1); // Adds component without initialization to the entity.

			world.Set(entity2, "String component"); // Adds component with specific value.

			world.Set(entity1, 10); // Overrides previously added component value.

			if (world.Has<string>(entity2)) // Checks whether the entity has such a component.
			{
				world.Remove<string>(entity2); // Removes a component from this entity.
			}

			ref int value = ref world.Get<int>(entity1); // Returns ref to component value.

			if (world.IsAlive(entity1)) // Checks whether the entity is alive.
			{
				world.Destroy(entity1); // Destroys this entity.
			}
		}
	}
}

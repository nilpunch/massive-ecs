namespace Massive.Samples.Basic
{
	class RegistryUsageSample
	{
		static void Main()
		{
			var registry = new Registry();

			var entity1 = registry.CreateEntity(); // Creates unique entity

			var entity2 = registry.CreateEntity<int>(); // Creates unique entity with a component

			registry.Assign<int>(entity1); // Assigns component with default value to an entity

			registry.Assign(entity2, "String component"); // Assigns component with specific value

			registry.Assign(entity1, 10); // Overrides previously assigned component value

			if (registry.Has<string>(entity2)) // Checks whether an entity has such a component
			{
				registry.Unassign<string>(entity2); // Unassigns a component from this entity
			}

			ref int value = ref registry.Get<int>(entity1); // Returns ref to component value

			if (registry.IsAlive(entity1)) // Checks whether an entity is alive
			{
				registry.Destroy(entity1); // Destroys this entity
			}
		}
	}
}

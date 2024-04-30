namespace Massive.Samples
{
	struct Position
	{
		public float X;
		public float Y;
	}

	struct Velocity
	{
		public float Magnitude;
	}

	class RegistryUsageSample
	{
		static void Update(IRegistry registry, float deltaTime)
		{
			var view = new View<Position, Velocity>(registry);

			// Iterate using view
			view.ForEach((int entity, ref Position position, ref Velocity velocity) =>
			{
				position.Y += velocity.Magnitude * deltaTime;

				if (position.Y > 5f)
				{
					// Create and destroy entities during iteration
					registry.Destroy(entity);
				}
			});

			// Pass extra arguments to avoid boxing
			view.ForEachExtra((registry, deltaTime),
				(int entity, ref Position position, ref Velocity velocity,
					(IRegistry Registry, float DeltaTime) passedArguments) =>
				{
					// ...
				});

			// Iterate manually over packed data, using Span<T>
			var velocities = registry.Components<Velocity>().Data;
			for (int i = 0; i < velocities.Length; ++i)
			{
				ref var velocity = ref velocities[i];
				// ...
			}
		}

		static void Main()
		{
			var registry = new MassiveRegistry();

			for (int i = 0; i < 10; ++i)
			{
				var entity = registry.Create();
				registry.Assign<Position>(entity, new Position() { X = i * 10f });
				if (i % 2 == 0)
				{
					registry.Assign<Velocity>(entity, new Velocity() { Magnitude = i * 10f });
				}
			}

			registry.SaveFrame();

			Update(registry, 1f / 60f);

			// Restore full state up to the last SaveFrame() call
			registry.Rollback(0);
		}
	}
}
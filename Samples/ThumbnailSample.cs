namespace Massive.Samples.Thumbnail
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

	class ThumbnailSample
	{
		static void Update(IRegistry registry, float deltaTime)
		{
			var view = registry.View<Position, Velocity>();

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

			// Iterate manually over data set
			var velocities = registry.Components<Velocity>();
			for (int i = 0; i < velocities.Count; ++i)
			{
				ref var velocity = ref velocities.Data[i];
				// ...
			}
		}

		static void Main()
		{
			var registry = new Registry();

			for (int i = 0; i < 10; ++i)
			{
				var entity = registry.Create();
				registry.Assign(entity, new Position() { X = i * 10f });
				if (i % 2 == 0)
				{
					registry.Assign(entity, new Velocity() { Magnitude = i * 10f });
				}
			}

			Update(registry, 1f / 60f);
		}
	}
}

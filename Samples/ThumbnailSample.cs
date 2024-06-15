namespace Massive.Samples.Thumbnail
{
	struct Player
	{
	}

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
		static IRegistry CreateRegistry()
		{
			var registry = new Registry();

			for (int i = 0; i < 10; ++i)
			{
				var entity = registry.Create(new Position() { X = i * 10f });

				if (i % 2 == 0)
					registry.Assign(entity, new Velocity() { Magnitude = i * 10f });

				if (i % 3 == 0)
					registry.Assign<Player>(entity);
			}

			return registry;
		}

		static void Update(IRegistry registry, float deltaTime)
		{
			var view = registry.View();

			// Iterate using views
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
				(ref Position position, ref Velocity velocity,
					(IRegistry Registry, float DeltaTime) args) =>
				{
					// ...
				});

			// Iterate manually over data sets
			var velocities = registry.Components<Velocity>();
			for (int i = 0; i < velocities.Count; ++i)
			{
				ref var velocity = ref velocities.Data[i];
				// ...
			}

			// Make queries right in place where they are used
			// You don't have to cache anything!
			registry.View()
				.Filter<Include<Player>, Exclude<Velocity>>()
				.ForEach((ref Position position) =>
				{
					// ...
				});
		}

		static void Main()
		{
			var registry = CreateRegistry();
			Update(registry, 1f / 60f);
		}
	}
}

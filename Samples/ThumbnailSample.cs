namespace Massive.Samples.Thumbnail
{
	struct Player { }
	struct Position { public float X; public float Y; }
	class Velocity { public float Magnitude; } // Classes work just fine
	delegate void ShootingMethod(); // So are the delegates
	interface IDontEvenAsk { }

	class Program
	{
		static void Main()
		{
			var registry = new Registry();

			// Create empty entity
			var enemy = registry.Create();

			// Or with a component
			var player = registry.Create(new Player());

			// Assign components
			registry.Assign(player, new Velocity() { Magnitude = 10f });
			registry.Assign(enemy, new Velocity());
			registry.Assign<Position>(enemy); // Assigns component without initialization

			// Get full entity identifier from player ID.
			// Handy when uniqueness is required, for example, when storing entities for later
			Entity playerEntity = registry.GetEntity(player);

			var deltaTime = 1f / 60f;

			// Iterate using lightweight views
			var view = registry.View();

			// Views will select only those entities that contain all the necessary components
			view.ForEach((int entityId, ref Position position, ref Velocity velocity) =>
			{
				position.Y += velocity.Magnitude * deltaTime;

				if (position.Y > 5f)
				{
					// Create and destroy any amount of entities during iteration
					registry.Destroy(entityId);
				}

				// NOTE:
				// After destroying any entities, refs to the components may be invalid in the current iteration.
				// If this behavior does not suit you, use IStable components.
			});

			// Pass extra arguments to avoid boxing
			view.ForEachExtra((registry, deltaTime),
				(ref Position position, ref Velocity velocity,
					(Registry Registry, float DeltaTime) args) =>
				{
					// ...
				});

			// Make queries right in place where they are used.
			// You don't have to cache anything
			registry.View()
				.Filter<Include<Player>, Exclude<Velocity>>()
				.ForEach((ref Position position) =>
				{
					// ...
				});

			// Iterate using foreach
			foreach (var entityId in registry.View().Include<Player, Position>())
			{
				ref var position = ref registry.Get<Position>(entityId);
				// ...
			}

			// Iterate manually over data sets
			var velocities = registry.DataSet<Velocity>();
			for (int i = 0; i < velocities.Count; ++i)
			{
				ref var velocity = ref velocities.Data[i];
				// ...
			}

			// Chain any amount of components in filters
			var filter = registry.Filter<
				Include<int, string, bool, Include<short, byte, uint, Include<ushort>>>,
				Exclude<long, char, float, Exclude<double>>>();

			// Reuse filter variable to reduce code duplication
			// in case of multiple iterations
			registry.View().Filter(filter).ForEach((ref int n, ref bool b) => { });
			registry.View().Filter(filter).ForEach((ref string str) => { });
		}
	}
}

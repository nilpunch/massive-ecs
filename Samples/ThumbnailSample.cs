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
			var world = new World();

			// Create empty entity
			var enemy = world.Create();

			// Or with a component
			var player = world.Create(new Player());

			// Assign components
			world.Assign(player, new Velocity() { Magnitude = 10f });
			world.Assign(enemy, new Velocity());
			world.Assign<Position>(enemy); // Assigns component without initialization

			// Get full entity identifier from player ID.
			// Handy when uniqueness is required, for example, when storing entities for later
			Entity playerEntity = world.GetEntity(player);

			var deltaTime = 1f / 60f;

			// Iterate using lightweight views
			var view = world.View();

			// Views will select only those entities that contain all the necessary components
			view.ForEach((int entityId, ref Position position, ref Velocity velocity) =>
			{
				position.Y += velocity.Magnitude * deltaTime;

				if (position.Y > 5f)
				{
					// Create and destroy any amount of entities during iteration
					world.Destroy(entityId);
				}

				// NOTE:
				// After destroying any entities, refs to the components may be invalid for the current iteration cycle.
				// If this behavior does not suit you, use IStable components
			});

			// Pass extra arguments to avoid boxing
			view.ForEachExtra((world: world, deltaTime),
				(ref Position position, ref Velocity velocity,
					(World World, float DeltaTime) args) =>
				{
					// ...
				});

			// Make queries right in place where they are used.
			// You don't have to cache anything
			world.View()
				.Filter<Include<Player>, Exclude<Velocity>>()
				.ForEach((ref Position position) =>
				{
					// ...
				});

			// Iterate using foreach
			foreach (var entityId in world.View().Include<Player, Position>())
			{
				ref var position = ref world.Get<Position>(entityId);
				// ...
			}

			// Iterate manually over data sets
			var velocities = world.DataSet<Velocity>();
			for (int i = 0; i < velocities.Count; ++i)
			{
				ref var velocity = ref velocities.Data[i];
				// ...
			}

			// Chain any amount of components in filters
			var filter = world.Filter<
				Include<int, string, bool, Include<short, byte, uint, Include<ushort>>>,
				Exclude<long, char, float, Exclude<double>>>();

			// Reuse filter variable to reduce code duplication
			// in case of multiple iterations
			world.View().Filter(filter).ForEach((ref int n, ref bool b) => { });
			world.View().Filter(filter).ForEach((ref string str) => { });
		}
	}
}

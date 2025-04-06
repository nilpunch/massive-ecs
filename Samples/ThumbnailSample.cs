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

			// Create empty entity.
			var enemy = world.Create();

			// Or with a component.
			var player = world.Create(new Player());

			// Add components.
			world.Add<Velocity>(player); // Adds component without initializing data.
			world.Get<Velocity>(player) = new Velocity() { Magnitude = 10f }; // Set the data.

			world.Set(enemy, new Velocity()); // Shortcut for the two operations above.

			// Get full entity identifier from player ID.
			// Handy when uniqueness is required, for example, when storing entities for later.
			Entity playerEntity = world.GetEntity(player);

			var deltaTime = 1f / 60f;

			// Iterate using lightweight views.
			var view = world.View();

			// Views will select only those entities that contain all the necessary components.
			view.ForEach((int entityId, ref Position position, ref Velocity velocity) =>
			{
				position.Y += velocity.Magnitude * deltaTime;

				if (position.Y > 5f)
				{
					// Create and destroy any amount of entities during iteration.
					world.Destroy(entityId);
				}

				// NOTE:
				// After destroying any entities, cached refs to components
				// may become invalid for the current interation cycle.
				// If this behavior does not suit you, use IStable components.
			});

			// Pass extra arguments to avoid boxing.
			view.ForEachExtra((world, deltaTime),
				(ref Position position, ref Velocity velocity,
					(World World, float DeltaTime) args) =>
				{
					// ...
				});

			// Filter entities right in place.
			// You don't have to cache anything.
			world.View()
				.Filter<Include<Player>, Exclude<Velocity>>()
				.ForEach((ref Position position) =>
				{
					// ...
				});

			// Iterate using foreach.
			var positions = world.DataSet<Position>();
			foreach (var entityId in world.View().Include<Player, Position>())
			{
				ref var position = ref positions.Get(entityId);
				// ...
			}

			// Chain any amount of components in filters.
			var filter = world.Filter<
				Include<int, string, bool, Include<short, byte, uint, Include<ushort>>>,
				Exclude<long, char, float, Exclude<double>>>();

			// Reuse filter variable to reduce code duplication
			// in case of multiple iterations.
			world.View().Filter(filter).ForEach((ref int n, ref bool b) => { });
			world.View().Filter(filter).ForEach((ref string str) => { });
		}
	}
}

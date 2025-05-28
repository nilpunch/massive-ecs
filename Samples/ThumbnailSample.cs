namespace Massive.Samples.Thumbnail
{
	struct Player { }
	struct Position { public float X; public float Y; }
	class Velocity { public float Magnitude; } // Classes work just fine.
	interface IDontEvenAsk { }

	class Program
	{
		static void Main()
		{
			// Create a world.
			var world = new World();

			// Create entities.
			var enemy = world.Create(); // Empty entity.
			var player = world.Create<Player>(); // With a component.

			// Add components.
			world.Add<Velocity>(player); // Adds component without initializing data.
			world.Get<Velocity>(player) = new Velocity() { Magnitude = 10f };

			world.Set(enemy, new Velocity()); // Adds component and sets its data.

			// Get full entity identifier from player ID.
			// Useful for persistent storage of entities.
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
				// may become invalid for the current iteration cycle.
				// If this behavior does not suit you, use Stable attribute on component.
			});

			// Pass arguments to avoid boxing.
			view.ForEach((world, deltaTime),
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

			// Iterate using foreach with data set.
			var positions = world.DataSet<Position>();
			foreach (var entityId in world.View().Include<Player, Position>())
			{
				ref Position position = ref positions.Get(entityId);
				// ...
			}

			// Chain any amount of components in filters.
			var filter = world.Filter<
				Include<int, string, bool, Include<short, byte, uint, Include<ushort>>>,
				Exclude<long, char, float, Exclude<double>>>();

			// Reuse filter variable to reduce code duplication.
			world.View().Filter(filter).ForEach((ref int n, ref bool b) => { });
			world.View().Filter(filter).ForEach((ref string str) => { });
		}
	}
}

# Massive ECS

<img align="right" width="180" height="180" src="https://github.com/user-attachments/assets/2a7bb2d3-75f1-43cd-8ac9-9ffb2edc0056" />

`Massive` is a lightweight and easy-to-use library for game programming and more.  
Designed for use in games with deterministic prediction-rollback netcode.  
Based on sparse sets. Inspired by [EnTT](https://github.com/skypjack/entt).

Does not reference Unity Engine, so it could be used in a regular C# project.

> [!NOTE]
> Some APIs are subject to change, but overall the architecture is stable.

## Installation

Make sure you have standalone [Git](https://git-scm.com/downloads) installed first. Reboot after installation.  
In Unity, open "Window" -> "Package Manager".  
Click the "+" sign at the top left corner -> "Add package from git URL..."  
Paste this: `https://github.com/nilpunch/massive-ecs.git#v18.1.0`  
See minimum required Unity version in the `package.json` file.

## Overview

This is **a library**, not a framework. Thus, it does not try to take control of the user codebase or the main game loop.

### Entity Component System ([wiki](https://github.com/nilpunch/massive-ecs/wiki/Entity-Component-System))

- Fast and simple.
- No code generation.
- No archetypes or bitsets.
- Fully managed, no unsafe code.
- Supports components of any type.
- No allocations and minimal memory consumption.
- No deferred command execution — all changes take effect immediately.
- Generic API for in-place filtering, with a non-generic version to improve IL2CPP build time and size.
- Lightweight [views](https://github.com/nilpunch/massive-ecs/wiki/Entity-Component-System#views) for safe and flexible iteration over entities and components.
- Fully stable storage on demand:
  - Use the `IStable` marker interface for components.
  - Or enable full stability for the entire world.
- IL2CPP friendly, tested with high stripping level on PC, Android, and WebGL.

### Rollbacks ([wiki](https://github.com/nilpunch/massive-ecs/wiki/Rollbacks))

- Fully optional and non-intrusive, integrates seamlessly with the existing ECS core.
- Minimalistic API: `SaveFrame()` and `Rollback(frames)`
- Supports components with managed data (e.g., arrays, strings, etc.).
- Performance reference (PC, CPU i7-11700KF, RAM 2666 MHz):  
  - 1000 entities, each with 150 components, can be saved 24 times in 6 ms.  
    The 150 components include 50 components of 64 bytes, 50 components of 4 bytes, and 50 tags.
  - Need more entities or reduced overhead? Adjust components or saving amount.  
    For example, 10000 entities with 15 components each can be saved 12 times in 2.3 ms.

### Addons

- Full-state serialization and deserialization ([package](https://github.com/nilpunch/massive-serialization)).
- Networking with input buffers, commands prediction, and resimulation loop ([package](https://github.com/nilpunch/massive-netcode)).
- Unity integration ([package](https://github.com/nilpunch/massive-unity-integration)).

Consider this list a work in progress as well as the project.

## Code Examples

```cs
struct Player { }
struct Position { public float X; public float Y; }
class Velocity { public float Magnitude; } // Classes work just fine.
delegate void ShootingMethod(); // So are the delegates.
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
		foreach (var entityId in world.View().Include<Player, Position>())
		{
			ref var position = ref world.Get<Position>(entityId);
			// ...
		}

		// Iterate manually over data sets.
		var velocities = world.DataSet<Velocity>();
		for (int i = 0; i < velocities.Count; ++i)
		{
			ref var velocity = ref velocities.Data[i];
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
```

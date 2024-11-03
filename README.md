# Massive ECS

A simple, easy to use ECS library, made for small fast paced online multiplayer games.  
Inspired by [EnTT](https://github.com/skypjack/entt).

> [!NOTE]
> Some APIs are subject to change, but overall the architecture is stable.

## Installation

Make sure you have standalone [Git](https://git-scm.com/downloads) installed first. Reboot after installation.  
In Unity, open "Window" -> "Package Manager".  
Click the "+" sign at the top left corner -> "Add package from git URL..."  
Paste this: `https://github.com/nilpunch/massive-ecs.git#v16.2.0`  
See minimum required Unity version in the `package.json` file.

## Overview

This is **a library**, not a framework. Thus, it does not try to take control of the user codebase or the main game loop.

[Here](https://github.com/nilpunch/bench-morpeh-leolite-massive) is a benchmark comparing Massive ECS with [Morpeh](https://github.com/scellecs/morpeh) and [LeoEcsLite](https://github.com/Leopotam/ecslite).  
TL;DR it's totally usable.

ECS features:

- No code generation
- Support сomponents of any type
- No deferred commands execution
- Garbage-free generic API for in-place queries
- Non-generic API for a better IL2CPP experience
- Fully stable storage on demand:
  - Use the `IStable` marker interface for components
  - Or enable full stability for the entire registry
- Groups for SoA multi-component iteration (inspired by [EnTT](https://github.com/skypjack/entt))
- Full state serialization and deserialization
- IL2CPP friendly, tested with high stripping level on PC | Android | WebGL
- [Unity integration](https://github.com/nilpunch/massive-unity-integration) (WIP)

Rollback features:

- This is an extension for ECS and is completely optional to use
- It's natively fast with Array.Copy and cyclic buffers
- Minimalistic API - `SaveFrame()` and `Rollback(frames)`
- Support for components with managed data, such as arrays, strings, etc. (see the [wiki](https://github.com/nilpunch/massive-ecs/wiki/Managed-components))

Consider this list a work in progress as well as the project.

## Code Examples

```cs
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
			// After destroying any entities, refs to the components may be invalid for the current iteration cycle.
			// If this behavior does not suit you, use IStable components
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
```

### How it works

Each *Massive* data structure contains cyclic buffer in linear memory. This allows for very fast saving and rollbacking, copying the entire data arrays at once. `MassiveRegistry` simply uses these *Massive* data structures internally, so we get the simplest possible ECS with rollbacks.

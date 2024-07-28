# Massive ECS - sparse set ECS with rollbacks

Made for games with deterministic prediction-rollback netcode. Currently distributed as Unity package.

Prediction-rollback netcode has very stable nature, and is mainly used in fast paced online multiplayer games, such as Overwatch and Rocket League.

## Overview

This is **a library**, not a framework. Thus, it does not try to take control of the user codebase or the main game loop.

ECS features:

- Fast and simple ECS without any code generation
- Support сomponents of any type
- No deferred commands execution
- Garbage-free API. Make in-place queries, no caching required!
- Groups for SoA multi-component iteration (inspired by [EnTT](https://github.com/skypjack/entt))
- Data pagination for stable resizing during iteration
- Full state serialization and deserialization
- IL2CPP friendly, tested with high stripping level on PC | Android | WebGL

Rollback features:

- Ultra-fast saving and rollbacking with cyclic buffers
- Support for components with managed data, such as arrays, strings, etc. (see the [wiki](https://github.com/nilpunch/massive-ecs/wiki/Managed-components))

[Unity integration](https://github.com/nilpunch/massive-unity-integration) (WIP)

Consider this list a work in progress as well as the project.

## Code Examples

```cs
using Massive;

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

class Program
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

		// Iterate using view
		view.ForEach((int entityId, ref Position position, ref Velocity velocity) =>
		{
			position.Y += velocity.Magnitude * deltaTime;

			if (position.Y > 5f)
			{
				// Create and destroy entities during iteration
				registry.Destroy(entityId);
			}
		});

		// Pass extra arguments to avoid boxing
		view.ForEachExtra((registry, deltaTime),
			(ref Position position, ref Velocity velocity,
				(IRegistry Registry, float DeltaTime) args) =>
			{
				// ...
			});

		// Make queries right in place where they are used
		// You don't have to cache anything!
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

		// Iterate manually over data set
		var velocities = registry.DataSet<Velocity>();
		for (int i = 0; i < velocities.Count; ++i)
		{
			ref var velocity = ref velocities.Data[i];
			// ...
		}
	}

	static void Main()
	{
		var registry = CreateRegistry();
		Update(registry, 1f / 60f);
	}
}
```

> [!NOTE]
> Some APIs are subject to change, but overall the architecture is stable.

## Installation

Make sure you have standalone [Git](https://git-scm.com/downloads) installed first. Reboot after installation.  
In Unity, open "Window" -> "Package Manager".  
Click the "+" sign at the top left corner -> "Add package from git URL..."  
Paste this: `https://github.com/nilpunch/massive-ecs.git`  
See minimum required Unity version in the `package.json` file.

### How it works

Each *Massive* data structure contains cyclic buffer in linear memory. This allows for very fast saving and rollbacking, copying the entire data arrays at once. `MassiveRegistry` simply uses these *Massive* data structures internally, so we get the simplest possible ECS with rollbacks.

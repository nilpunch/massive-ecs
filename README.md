# Massive ECS

<img align="right" width="180" height="180" src="https://github.com/user-attachments/assets/2a7bb2d3-75f1-43cd-8ac9-9ffb2edc0056" />

`Massive` is a fast and simple library for game programming and more.  
Designed for use in games with deterministic prediction-rollback netcode.  
Based on sparse sets. Inspired by [EnTT](https://github.com/skypjack/entt).

Does not reference Unity Engine, so it could be used in a regular C# project.

> [!NOTE]
> Some APIs are subject to change, but overall the architecture is stable.

## Installation

Make sure you have standalone [Git](https://git-scm.com/downloads) installed first. Reboot after installation.  
In Unity, open "Window" -> "Package Manager".  
Click the "+" sign at the top left corner -> "Add package from git URL..."  
Paste this: `https://github.com/nilpunch/massive-ecs.git#v19.2.0`  
See minimum required Unity version in the `package.json` file.

## Overview

This is **a library**, not a framework. Thus, it does not try to take control of the user codebase or the main game loop.

It’s organized as a set of loosely coupled containers that can be used in different ways.

### Entity Component System ([wiki](https://github.com/nilpunch/massive-ecs/wiki/Entity-Component-System))

Design considerations:

- Deterministic with lazy initialization.
- Component definition is configuration — no extra boilerplate.
- No deferred command execution — all changes apply immediately.
- Minimal storage for fast saving. No archetypes or bitsets.
- Fully managed — no unsafe code, no `Dispose()` methods.
- Supports components of any type.
- IL2CPP friendly, tested with high stripping level on PC, Android, and WebGL.

Features:

- `Clone()` and `CopyTo(other)` methods for creating snapshots.  
  Ideal for implementing replays, undo/redo, or rollbacks.
- Lightweight [views](https://github.com/nilpunch/massive-ecs/wiki/Entity-Component-System#views) for adaptive iteration over entities and components.
- Fully stable storage (no reference invalidation) on demand:
  - Use the `Stable` attribute for components.
  - Or enable full stability for the entire world.
- `Allocator` lets you use collections inside components and easily integrate external tools in rollback pipeline.

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
interface IDontEvenAsk { }

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
// The world itself acts as a view too.
var view = world.View();

// ForEach will select only those entities that contain all the necessary components.
world.ForEach((int entityId, ref Position position, ref Velocity velocity) =>
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
world.ForEach((world, deltaTime),
	(ref Position position, ref Velocity velocity,
		(World World, float DeltaTime) args) =>
	{
		// ...
	});

// Filter entities right in place.
// You don't have to cache anything.
world.Filter<Include<Player>, Exclude<Velocity>>()
	.ForEach((ref Position position) =>
	{
		// ...
	});

// Iterate using foreach with data set.
var positions = world.DataSet<Position>();
foreach (var entityId in world.Include<Player, Position>())
{
	ref Position position = ref positions.Get(entityId);
	// ...
}

// Chain any number of components in filters.
var filter = world.Filter<
	Include<int, string, bool, Include<short, byte, uint, Include<ushort>>>,
	Exclude<long, char, float, Exclude<double>>>();

// Reuse the same filter view to iterate over different components.
filter.ForEach((ref int n, ref bool b) => { });
filter.ForEach((ref string str) => { });
```

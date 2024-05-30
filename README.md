# Massive ECS - sparse set ECS with rollbacks

Made for games with deterministic prediction-rollback netcode. Currently distributed as Unity package.

Prediction-rollback netcode has very stable nature, and is mainly used in fast paced online multiplayer games, such as Overwatch and Rocket League.

## Overview

This is **a library**, not a framework. Thus, it does not try to take control of the user codebase or the main game loop.

Provided features:

- Fast and simple ECS without any code generation
- Ultra-fast saving and rollbacking with cyclic buffers
- Zero GC allocations during runtime (after full initialization)
- Support for components with managed data, such as arrays, strings, etc. (see the [wiki](https://github.com/nilpunch/massive-ecs/wiki/Managed-components))
- Groups for SoA multi-component iteration (inspired by [EnTT](https://github.com/skypjack/entt))
- Data pagination for stable resizing during iteration
- Full state serialization
- IL2CPP friendly, tested with high stripping level on PC | Android | WebGL
- [Unity integration](https://github.com/nilpunch/massive-unity-integration) (WIP)

Consider this list a work in progress as well as the project.

## Code Examples

```cs
using Massive;

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
    static void Update(IRegistry registry, float deltaTime)
    {
        var view = new View<Position, Velocity>(registry);

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
        var registry = new MassiveRegistry();

        for (int i = 0; i < 10; ++i)
        {
            var entity = registry.Create();
            registry.Assign<Position>(entity, new Position() { X = i * 10f });
            if (i % 2 == 0)
            {
                registry.Assign<Velocity>(entity, new Velocity() { Magnitude = i * 10f });
            }
        }

        registry.SaveFrame();

        Update(registry, 1f / 60f);

        // Restore full state up to the last SaveFrame() call
        registry.Rollback(0);
    }
}
```

## Installation

Make sure you have standalone [Git](https://git-scm.com/downloads) installed first. Reboot after installation.  
In Unity, open "Window" -> "Package Manager".  
Click the "+" sign at the top left corner -> "Add package from git URL..."  
Paste this: `https://github.com/nilpunch/massive-ecs.git`  
See minimum required Unity version in the `package.json` file.

## Types Overview

> [!NOTE]
> Some APIs are subject to change, but overall the architecture is stable.

You can just use these types if you don't need the rollback functionality:

- `SparseSet` - data structure containing ids, similar to `HashSet<int>`
- `DataSet<T>` - data extension for `SparseSet`, similar to `Dictionary<int, T>`
- `Entities` - data structure to generate and reuse entities - unique ids
- `Registry` - container for entities, components and tags

Each type has a *Massive* counterpart with added rollback functionality:

- `MassiveSparseSet` - counterpart to `SparseSet`
- `MassiveDataSet<T>` - counterpart to `DataSet<T>`
- `MassiveManagedDataSet<T>` - same as `MassiveDataSet<T>`, but with customizable saving
- `MassiveEntities` - counterpart to `Entities`
- `MassiveRegistry` - counterpart to `Registry`

### How it works

Each *Massive* data structure contains cyclic buffer in linear memory. This allows for very fast saving and rollbacking, copying the entire data arrays at once. `MassiveRegistry` simply uses these *Massive* data structures internally, so we get the simplest possible ECS with rollbacks.

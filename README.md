# Massive ECS - sparse set ECS with rollbacks

Made for games with deterministic prediction-rollback netcode. Currently distributed as Unity package.

Prediction-rollback netcode have very stable nature, and is mainly used in fast paced online multiplayer games, such as Overwatch and Rocket League.

## Overview

This is **a library**, not a framework. Thus, it does not try to take control of the user codebase or the main game loop.

Provided features:

- Fast and simple ECS without any code generation
- Ultra-fast saving and rollbacking with cyclic buffers
- Zero GC allocations during runtime (after full initialization)
- Support for components with managed data, such as arrays, strings, etc. (see the [wiki](https://github.com/nilpunch/massive-ecs/wiki/Managed-components))
- Groups for SoA multi-component iteration (inspired by [EnTT](https://github.com/skypjack/entt))
- Full-state serialization
- IL2CPP friendly, tested on PC | Android | WebGL

Work in progress:

- User-friendly [integration with Unity](https://github.com/nilpunch/unity-prediction-rollback)

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

        // Iterate manually over packed data, using Span<T>
        var velocities = registry.Components<Velocity>().Data;
        for (int i = 0; i < velocities.Length; ++i)
        {
            ref var velocity = ref velocities[i];
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

You can use just them if you don't need rollback functionality:

- `SparseSet` - data structure containing ids, similar to `HashSet<int>`
- `DataSet<T>` - data extension for `SparseSet`, similar to `Dictionary<int, T>`
- `ManagedDataSet<T>` - same as `DataSet<T>`, but with customizable saving
- `Entities` - data structure to generate and reuse ids
- `Registry` - container for entities, components and tags

Each type has a *Massive* counterpart with added rollback functionality:

- `MassiveSparseSet` - counterpart to `SparseSet`
- `MassiveDataSet<T>` - counterpart to `DataSet<T>`
- `MassiveManagedDataSet<T>` - counterpart to `ManagedDataSet<T>`
- `MassiveEntities` - counterpart to `Entities`
- `MassiveRegistry` - counterpart to `Registry`

### How it works

Each *Massive* data structure contains cyclic buffer in linear memory. This allows for very fast saving and rollbacking, copying the entire data arrays at once. `MassiveRegistry` simply uses these *Massive* data structures internally, so we get the simplest possible ECS with rollbacks.

## Test Samples

### Shooter

Over 1000 bullets resimulating 120 frames per one frame at 60 FPS simulation.

https://github.com/nilpunch/massive/assets/69798762/45f3011b-db48-4fa9-a6cf-9cf101d11cc7

That is 1000 \* 120 = 120 000 updates per frame, and 120 000 \* 60 = 7 200 000 updates per second.

Note that simulating 1000 bullets for 120 frames is **not the same** as simulating 120 000 bullets for one frame. The last case is highly parallelizable, while the resimulation case is sequential and in terms of computation can only be optimised via SIMD.

### Physics

Simple rigidbody physics (just a showcase, not well optimised for resimulations because of brute force collision detection)

https://github.com/nilpunch/massive/assets/69798762/53779ac1-fb6a-44ed-9e35-fb9ce30df6a5

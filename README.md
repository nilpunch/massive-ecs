# Sparse set library, featuring rollbacks

> [!NOTE]
> Basic sparse set ECS included!

Made for competitive and fast paced online multiplayer games with prediction-rollback netcode.

## How to use

This is **a library**, not a framework. Thus, it doesn't try to take control of the user codebase or the main game loop.

Main types overview. You can use them if you don't need rollback functionality:

- `SparseSet` - data structure similar to `HashSet<int>`
- `DataSet<T>` - data wrapper for `SparseSet`, similar to `Dictionary<int, T>`
- `Registry` - container for entities, components and tags

Each type has a *Massive* counterpart with rollback functionality:

- `MassiveSparseSet` - counterpart to `SparseSet`
- `MassiveDataSet<T>` - counterpart to `DataSet<T>`
- `MassiveRegistry` - counterpart to `Registry`

## How it works

Each *Massive* data structure contains linear cyclic buffer. This allows for very fast saving and rollbacking, copying the entire data arrays at once. `MassiveRegistry` simply uses these *Massive* data structures internally, so we get the simplest possible ECS with rollbacks.

## Samples

### Shooter

Over 1000 bullets resimulating 120 frames per one frame at 60 FPS simulation.

https://github.com/nilpunch/massive/assets/69798762/45f3011b-db48-4fa9-a6cf-9cf101d11cc7

That is 1000\*120 = 120.000 updates per frame, and 120.000\*60 = 7.200.000 updates per second.

Note that simulating 1000 bullets for 120 frames is **not the same** as simulating 120.000 bullets for one frame. The last case is highly parallelizable, while the resimulation case is sequential and in terms of computation can only be optimised via SIMD.

### Physics

Simple rigidbody physics (just a showcase, not well optimised for resimulations because of brute force collision detection)

https://github.com/nilpunch/massive/assets/69798762/53779ac1-fb6a-44ed-9e35-fb9ce30df6a5

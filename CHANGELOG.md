# Changelog

## 15.0.0 - October 18, 2024

Sparse set mods rework.

- Changed: Renamed `IndexingMode.Packed` to `PackingMode.Continuous`.
- Changed: Replaced `IndexingMode.Direct` with `PackingMode.WithHoles`.
  The new behavior creates holes in the packed array on Unassign(), which are reused during Assign().
  Unlike the previous version, this approach naturally tends toward storage compaction over time.

## 14.0.0 - October 16, 2024

Major stability update.

- Added: `IndexingMode` for sparse sets, that allows to choose between packed (indirect) and direct storaging.
  `IndexingMode.Packed` represents the previous behavior of sets and is the default.
  Even when `IndexingMode` is `Packed`, now you can delete and create entities much more freely during iteration.
- Added: `IStable` marker interface for components, that must be stored with `IndexingMode.Direct`.
- Added: `First()`, `FirstEntity()`, `Destroy()`, `Count()` extension methods for views.
- Changed: `IEntityAction.Apply` now returns bool and can break from iteration.
- Changed: Renamed all "Dense" stuff to "Packed".
- Changed: Massives now sync only the current frame capacity during saving.
  This will allow more gradual memory allocation.

## 13.0.0 - August 25, 2024

- Changed: Divided `Assign<T>()` into two distinct overloads - with data initialization and without it.
  This will allow the sets to be used as objects pools.
- Changed: Order of groups to more common pattern `Group<Include, Exclude, Owned>`.

## 12.0.0 - August 10, 2024

Devirtualize usages for better inlining and performance.

- Changed: Replaced `ISet` and `IReadOnlySet` with `SparseSet`.
- Changed: Replaced `IDataSet<T>` and `IReadOnlyDataSet<T>` with `DataSet<T>`.
- Changed: Replaced `IRegistry` with `Registry`.
- Changed: Replaced `IEntities` with `Entities`.
- Removed: `IEntities` and `IRegistry` interfaces.

## 11.0.0 - July 28, 2024

Refactoring and some API change.

- Changed: Renamed registry `Components<T>` extension method to `DataSet<T>`.
- Changed: Renamed registry `Any<T>` extension method to `Set<T>`.

## 10.0.0 - July 1, 2024

Minor API renaming, but major up. None the less.

- Changed: Renamed `IView` methods.
- Changed: Renamed `Invokers` to `Actions`.

## 9.3.1 - June 16, 2024

- Changed: Renamed `EntityFillInvoker` and `EntityFillEntitiesInvoker` to `FillIdsInvoker` and `FillEntitiesInvoker`.

## 9.3.0 - June 16, 2024

- Changed: Removed final bits of unsafe code from the project.

## 9.2.0 - June 16, 2024

- Added: Id enumerators for the views.

## 9.1.0 - June 15, 2024

- Added: `Include()` and `Exclude()` extension methods for basic `View`. That simplifies querying.

## 9.0.0 - June 15, 2024

- Changed: Major redesign of the views API for ease of use and extensibility.
- Removed: `Fill()` extension methods from `IRegistry`. Views still have them.

## 8.1.1 - June 15, 2024

- Optimized: Added funky bitwise stuff into filters, so now they are working 20% faster.

## 8.1.0 - June 14, 2024

- Added: `Fill()` extension methods for `IRegistry` to populate the result list with filtered ids and entities.

## 8.0.0 - June 14, 2024

- Changed: Removed `SetRegistry` parameter from `Get()` method in `FilterRegistry` and `GroupRegistry`.
  Now it is proper dependency.
- Removed: `MassiveGroupRegistry` has been removed, and now `MassiveRegistry` is responsible for saving and rollbacking groups.
- Changed: Reworked protected `Registry` ctor to fit API changes.

## 7.3.2 - June 14, 2024

- Changed: Set selectors now return distinct sets. This is useful when the user duplicates generic arguments.
  We can't prevent this from happening at compile time, so now at least it doesn't affect runtime.

## 7.3.1 - June 14, 2024

- Changed: Items sorting in `GenericLookup` now works with full type names instead of type names hashes,
  which eliminates possible problems related to hash collisions.

## 7.3.0 - June 12, 2024

- Added: `ActionRef` delegates similar to `EntityActionRef`, but without *id* parameter.
- Added: `ForEach()` and `ForEachExtra()` extension methods overloads with `ActionRef` and `ActionRefExtra` parameters for `IView`.

## 7.2.0 - June 12, 2024

- Added: `Clone()` extension method for `IRegistry` to clone entities.

## 7.1.0 - June 11, 2024

- Added: `Fill()` extension method for `IView` to populate the result list with filtered ids.

## 7.0.0 - June 9, 2024

- Changed: Replaced `IRegistry` parameter in views ctors with fields initialization.

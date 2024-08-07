# Changelog

## 11.0.0 - July 28, 2024

Refactoring and some API change.

- Changed: renamed registry `Components<T>` extension method to `DataSet<T>`.
- Changed: renamed registry `Any<T>` extension method to `Set<T>`.

## 10.0.0 - July 1, 2024

Minor API renaming, but major up. None the less.

- Changed: renamed `IView` methods.
- Changed: renamed `Invokers` to `Actions`.

## 9.3.1 - June 16, 2024

- Changed: renamed `EntityFillInvoker` and `EntityFillEntitiesInvoker` to `FillIdsInvoker` and `FillEntitiesInvoker`.

## 9.3.0 - June 16, 2024

- Changed: removed final bits of unsafe code from the project.

## 9.2.0 - June 16, 2024

- Added: id enumerators for the views.

## 9.1.0 - June 15, 2024

- Added: `Include()` and `Exclude()` extension methods for basic `View`. That simplifies querying.

## 9.0.0 - June 15, 2024

- Changed: major redesign of the views API for ease of use and extensibility.
- Removed: `Fill()` extension methods from `IRegistry`. Views still have them.

## 8.1.1 - June 15, 2024

- Optimized: added funky bitwise stuff into filters, so now they are working 20% faster.

## 8.1.0 - June 14, 2024

- Added: `Fill()` extension methods for `IRegistry` to populate the result list with filtered ids and entities.

## 8.0.0 - June 14, 2024

- Changed: removed `SetRegistry` parameter from `Get()` method in `FilterRegistry` and `GroupRegistry`.
  Now it is proper dependency.
- Removed: `MassiveGroupRegistry` has been removed, and now `MassiveRegistry` is responsible for saving and rollbacking groups.
- Changed: reworked protected `Registry` ctor to fit API changes.

## 7.3.2 - June 14, 2024

- Changed: set selectors now return distinct sets. This is useful when the user duplicates generic arguments.
  We can't prevent this from happening at compile time, so now at least it doesn't affect runtime.

## 7.3.1 - June 14, 2024

- Changed: items sorting in `GenericLookup` now works with full type names instead of type names hashes,
  which eliminates possible problems related to hash collisions.

## 7.3.0 - June 12, 2024

- Added: `ActionRef` delegates similar to `EntityActionRef`, but without *id* parameter.
- Added: `ForEach()` and `ForEachExtra()` extension methods overloads with `ActionRef` and `ActionRefExtra` parameters for `IView`.

## 7.2.0 - June 12, 2024

- Added: `Clone()` extension method for `IRegistry` to clone entities.

## 7.1.0 - June 11, 2024

- Added: `Fill()` extension method for `IView` to populate the result list with filtered ids.

## 7.0.0 - June 9, 2024

- Changed: replaced `IRegistry` parameter in views ctors with fields initialization.

# Changelog

## 8.0.0 - June 14, 2024

- Changed: removed `SetRegistry` parameter from `Get()` method in `FilterRegistry` and `GroupRegistry`.
  Now it is proper dependency.
- Removed: `MassiveGroupRegistry` has been removed, and now `MassiveRegistry` is responsible for saving and rollbacking groups.
- Changed: reworked protected `Registry` ctor to fit API changes.

## 7.3.2 - June 14, 2024

- Changed: set selectors now return distinct sets. This is useful when the user duplicates generic arguments.
  We can't prevent this from happening at compile time, so now at least it doesn't affect runtime.

## 7.3.1 - June 14, 2024

- Changed: items sorting in `GenericLookup` now works with full type names instead of type names hashes, which eliminates possible problems related to hash collisions.

## 7.3.0 - June 12, 2024

- Added: `ActionRef` delegates similar to `EntityActionRef`, but without *id* parameter.
- Added: `ForEach()` and `ForEachExtra()` extension methods overloads with `ActionRef` and `ActionRefExtra` parameters for `IView`.

## 7.2.0 - June 12, 2024

- Added: `Clone()` extension method for `IRegistry` to clone entities.

## 7.1.0 - June 11, 2024

- Added: `Fill()` extension method for `IView` to populate the result list with filtered ids.

## 7.0.0 - June 9, 2024

- Changed: replaced `IRegistry` parameter in views ctors with fields initialization.

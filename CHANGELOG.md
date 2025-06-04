# Changelog

## Unreleased

### Changed

- Renamed `SetRegistry` to `Sets`.
- Renamed `FilterRegistry` to `Filters`.
- Renamed `AllocatorRegistry` to `Allocators`.
- `Destroy()` no longer throws when called with a dead entity.
- Replaced `MassiveAssert` with individual exceptions.
- `World.Set()` now throws if the component has no associated data.
- Renamed `World.Filter()` to `World.GetFilter()`.
- Made `World` act as a `View` to reduce boilerplate.

## 20.0.0-alpha.2 - April 27, 2025

### Added

- `Allocator`, enabling fast rollbacks and serialization for collections inside components,
  and allowing external tools to plug directly into the data pipeline of the `World`.

### Changed

- Renamed data sets based on their usage:
  - Renamed `ResettingDataSet` to `UnmanagedDataSet`.
  - Renamed `SwappingDataSet` to `ManagedDataSet`.
- Renamed `Assert` to `MassiveAssert`.
- Reworked `default(Entity)` behaviour:
  - Entity ID is now stored without offset.
  - Alive entities start with version 1.
  - Entities with version 0 are now considered dead.

## 20.0.0-alpha.1 - April 27, 2025

### Fixed

- `View` and `FilterView` sometimes choose and use not optimal minimal set.

## 20.0.0-alpha - April 27, 2025

### Added

- `DefaultValue` attribute to customize the reset value for unmanaged components.

## Changed

- Renamed `ForEachExtra` to `ForEach`, making it just another overload.
- Renamed `MaxId` to `UsedIds` in `Entities` for consistency with `SparseSet`.

## 19.1.0 - April 16, 2025

### Added

- `PageSize` attribute to override the default page size for a data set of the specified type.

### Changed

- Replaced the `IStable` marker interface with a `Stable` attribute.

## 19.0.0 - April 5, 2025

BREAKING CHANGE: Major renaming, groups removal.

### Changed

- Renamed `Registry` class to `World`.
- Changed sets API:
  - Renamed `Assign(id)` to `Add(id)`.
  - Renamed `Unassign(id)` to `Remove(id)`.
  - Renamed `IsAssigned(id)` to `Has(id)`.
  - Renamed `Assign(id, data)` to `Set(id, data)`.
  - `Add(id)` and `Remove(id)` returns `true` if the component is added or removed
    and `false` if it is already present or not there.
- Added assertions for negative arguments in some methods.

### Added

- `Clone()` and `CopyTo(other)` methods to `SparseSet`, `DataSet<T>` and `World`.

### Removed

- Groups.

## 18.1.0 - February 24, 2025

### Added

- Assertions for numerous execution paths.

### Changed

- `Destroy`, `Assign`, `Unassign`, `Has` and `Clone` methods now disallow passing dead entities or IDs.
- `Filter.Empty` static property has been moved to `FilterRegistry` and is now non-static.

## 18.0.0 - January 22, 2025

### Fixed

- `Entities.HasHoles` worked incorrectly for `Packing.WithPersistentHoles`.

## 18.0.0-alpha - January 7, 2025

### Changed

- Empty type optimization (tags) now only applies to value types, instead of any type without fields.
- Renamed `TrimmedFilter` to `ReducedFilted` and tweak some internal method names.
- Reworked `TypeIdentifier` and `GenericLookup`.
- Optimize `FilterView.ForEach()` methods for cases where a minimal set is one of the selected data sets.
- Moved serialization to a separate package to eliminate unsafe code from the core library.
- `PageSequence` now use `Page` struct to iterate.
- `MassiveRegistry` now saves first empty for `MassiveEntities` on creation.

### Fixed

- IL2CPP stripping for `ICopyable<T>`.

## 17.0.0 - December 9, 2024

Iteration stability, sets customization, groups rework, performance improvements and more.

### Added

- Copy, move, and swap customization for data sets.
- `DynamicFilter` for builder-like style when working with non-generic filtering.
- `TrimmedFilter`, which removes the leading set from included ones, optimizing iteration.
- New packing option `WithPersistentHoles`.

### Changed

- Renamed `PackingMode` to `Packing`.
- `Packing` now dynamically changes for the iteration leader,  
  ensuring each unique entity appears only once during iteration in most cases,  
  or in all cases when using the new packing option.
- Hide setters in data structures. Added full-state assignment API.
- Renamed `IdsSource` to `PackedSet`.
- Renamed `IManaged<T>` to `ICopyable<T>`. Now used also for entity components cloning.
- Renamed `NonOwningGroup` to `Group`.

### Removed

- `OwningGroup` for SoA access. It may return in some form in the future.
- `GroupView`. Groups are now used directly as is.
- `GroupPage`, `GroupPageSequence` and `IOwnSelector`.
- Runtime/Utils folder.
- `RegistryReflectionExtensions`. Reflection methods remain directly accessible.

### Fixed

- `Il2CppSetOption` was not applied correctly.

## 16.3.0 - November 7, 2024

### Added

- `PackingMode` to `Entities`.
- `PackingMode` now can be changed at runtime both for `Entities` and `SparseSet`.

### Changed

- `GroupPage` now returns `Entry`, containing both in-page index and ID.

### Removed

- `Alive` span property from `Entities`. Use enumerator instead.
- Delegate parameter from `Entities.CreateMany`.

## 16.2.0 - November 3, 2024

### Added

- `Clear` method for `Entities`.
- `Clear` and `Clear<T>` extension methods for `Registry`.
- Enumerator for `Group`.

### Changed

- Renamed `Include` and `Exclude` properties to `Included` and `Excluded`.  
  This change affects filters, groups, and some variables.

### Removed

- `AsIds` extension method for groups. Use the new enumerator instead.

## 16.1.0 - November 2, 2024

Non-generic API for a better IL2CPP experience.

### Added

- Non-generic version of the `Get` method for `GroupRegistry`, making it the primary version.
- `GroupPageSequence` and `GroupPage` types to support working with owning groups in a non-generic manner.
- Enumerator for `SparseSet` and `Entities`.
- `PageSize` property to `Registry`.

### Changed

- `Entities.AfterCreated` now returns an `int` instead of an `Entity`.
- Renamed `RegistryConfig.DataPageSize` to `PageSize`.
- Group serialization adjusted to align with the new non-generic primary storage.

### Fixed

- Non-owning groups were not functioning when no included sets were present.
  Now they default to observing `Entities`.

## 16.0.0 - October 30, 2024

Major devirtualization and other optimizations.

### Added

- Fourth generic parameter for all Views.
- `Group` class, replacing the `IGroup` interface.
- `IdsSource` class, replacing the `IIdsSource` interface.

### Changed

- Replaced `IFilter` and other filters with `Filter`.
- Replaced `IGroup` and `IOwningGroup` with `Group` and `OwningGroup` classes.
- Increased default page size from 1024 to 16384 (except `MassiveRegistry`, which remains 1024).

### Removed

- `IGroup`, `IFilter`, and `IIdsSource` interfaces.
- `SetCapacity` parameter; now defaults to 0 across the board.

## 15.3.2 - October 28, 2024

### Changed

- Replaced `ArraySegment` with direct array usage to improve performance.

### Fixed

- `PageSequence` returned zero-length page in specific cases.
- Non-serialized sets and groups not being cleared after deserialization.

## 15.3.1 - October 27, 2024

### Fixed

- Forgot to serialize the pointer to the next hole in the sparse set.
- Removed unnecessary resizes during deserialization to reduce allocations.

## 15.3.0 - October 25, 2024

Major serialization rework and addition of reflection toolset.

### Changed

- RegistrySerializer no longer requires manual setup and works automatically in most cases.
  Customization remains available.
- Renamed `IDataSetSerializer` to `IDataSerializer` with slight API modifications.

### Added

- Support for `Type` argument in addition to generic ones for:
  - ISetFactory.CreateAppropirateSet(Component)
  - SetRegistry.Get(Component)
  - GroupRegistry.Get(Include,Exclude,Own)
- Methods to work with raw data for `IDataSet`.
- Non-generic `IPagedArray` interface for `PagedArray<T>`.

### Fixed

- `Create<T>()` and `CreateEntity<T>()` now do not initialize data, aligning with `Assign<T>()` behavior.

## 15.2.0 - October 19, 2024

### Added

- `FullStability` option for Registry.

## 15.1.0 - October 19, 2024

Remove some abstractions and add some minor things.

### Added

- `Compact()` method to sparse set, which removes all holes from the packed array.
- `DefaultPackingMode` field into `RegistryConfig`.

### Changed

- Renamed `IGroup.Set` to `IGroup.MainSet`.

### Removed

- `MassiveDataSetBase`, leaving only `MassiveDataSet` and `MassiveManagedDataSet`.
- `ISet`, `IReadOnlySet`, `IDataSet<T>` and `IReadOnlyDataSet<T>` interfaces.

## 15.0.0 - October 18, 2024

Sparse set mods rework.

### Changed

- Renamed `IndexingMode.Packed` to `PackingMode.Continuous`.
- Replaced `IndexingMode.Direct` with `PackingMode.WithHoles`.
  The new behavior creates holes in the packed array on Unassign(), which are reused during Assign().
  Unlike the previous version, this approach naturally tends toward storage compaction over time.

## 14.0.0 - October 16, 2024

Major stability update.

### Added

- `IndexingMode` for sparse sets, that allows to choose between packed (indirect) and direct storaging.
  `IndexingMode.Packed` represents the previous behavior of sets and is the default.
  Even when `IndexingMode` is `Packed`, now you can delete and create entities much more freely during iteration.
- `IStable` marker interface for components, that must be stored with `IndexingMode.Direct`.
- `First()`, `FirstEntity()`, `Destroy()`, `Count()` extension methods for views.

### Changed

- `IEntityAction.Apply` now returns bool and can break from iteration.
- Renamed all "Dense" stuff to "Packed".
- Massives now sync only the current frame capacity during saving.
  This will allow more gradual memory allocation.

## 13.0.0 - August 25, 2024

### Changed

- Divided `Assign<T>()` into two distinct overloads - with data initialization and without it.
  This will allow the sets to be used as objects pools.
- Order of groups to more common pattern `Group<Include, Exclude, Owned>`.

## 12.0.0 - August 10, 2024

Devirtualize usages for better inlining and performance.

### Changed

- Replaced `ISet` and `IReadOnlySet` with `SparseSet`.
- Replaced `IDataSet<T>` and `IReadOnlyDataSet<T>` with `DataSet<T>`.
- Replaced `IRegistry` with `Registry`.
- Replaced `IEntities` with `Entities`.

### Removed

- `IEntities` and `IRegistry` interfaces.

## 11.0.0 - July 28, 2024

Refactoring and some API change.

### Changed

- Renamed registry `Components<T>` extension method to `DataSet<T>`.
- Renamed registry `Any<T>` extension method to `Set<T>`.

## 10.0.0 - July 1, 2024

Minor API renaming, but major up. None the less.

### Changed

- Renamed `IView` methods.
- Renamed `Invokers` to `Actions`.

## 9.3.1 - June 16, 2024

### Changed

- Renamed `EntityFillInvoker` and `EntityFillEntitiesInvoker` to `FillIdsInvoker` and `FillEntitiesInvoker`.

## 9.3.0 - June 16, 2024

### Changed

- Removed final bits of unsafe code from the project.

## 9.2.0 - June 16, 2024

### Added

- Id enumerators for the views.

## 9.1.0 - June 15, 2024

### Added

- `Include()` and `Exclude()` extension methods for basic `View`. That simplifies querying.

## 9.0.0 - June 15, 2024

### Changed

- Major redesign of the views API for ease of use and extensibility.

### Removed

- `Fill()` extension methods from `IRegistry`. Views still have them.

## 8.1.1 - June 15, 2024

### Changed

- Added funky bitwise stuff into filters, so now they are working 20% faster.

## 8.1.0 - June 14, 2024

### Added

- `Fill()` extension methods for `IRegistry` to populate the result list with filtered ids and entities.

## 8.0.0 - June 14, 2024

### Changed

- Removed `SetRegistry` parameter from `Get()` method in `FilterRegistry` and `GroupRegistry`.
  Now it is proper dependency.
- Reworked protected `Registry` ctor to fit API changes.

### Removed

- `MassiveGroupRegistry` has been removed, and now `MassiveRegistry` is responsible for saving and rollbacking groups.

## 7.3.2 - June 14, 2024

### Changed

- Set selectors now return distinct sets. This is useful when the user duplicates generic arguments.
  We can't prevent this from happening at compile time, so now at least it doesn't affect runtime.

## 7.3.1 - June 14, 2024

### Changed

- Items sorting in `GenericLookup` now works with full type names instead of type names hashes,
  which eliminates possible problems related to hash collisions.

## 7.3.0 - June 12, 2024

### Added

- `ActionRef` delegates similar to `EntityActionRef`, but without *id* parameter.
- `ForEach()` and `ForEachExtra()` extension methods overloads with `ActionRef` and `ActionRefExtra` parameters for `IView`.

## 7.2.0 - June 12, 2024

### Added

- `Clone()` extension method for `IRegistry` to clone entities.

## 7.1.0 - June 11, 2024

### Added

- `Fill()` extension method for `IView` to populate the result list with filtered ids.

## 7.0.0 - June 9, 2024

### Changed

- Replaced `IRegistry` parameter in views ctors with fields initialization.

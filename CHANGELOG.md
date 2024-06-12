# Changelog

## 7.3.0 - June 12, 2024

- Added: `ActionRef` delegates similar to `EntityActionRef`, but without *id* parameter.
- Added: `ForEach()` and `ForEachExtra()` extension methods overloads with `ActionRef` and `ActionRefExtra` parameters for `IView`.

## 7.2.0 - June 12, 2024

- Added: `Clone()` extension method for `IRegistry` to clone entities.

## 7.1.0 - June 11, 2024

- Added: `Fill()` extension method for `IView` to populate the result list with filtered ids.

## 7.0.0 - June 9, 2024

- Changed: replaced `IRegistry` parameter in views ctors with fields initialization.

#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public static class MassiveAssert
	{
		public const string Symbol = "MASSIVE_ASSERT";
		public const string Library = "[MASSIVE]";

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void That(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception(message);
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NonNegative(int value)
		{
			if (value < 0)
			{
				throw new Exception($"{Library} Provided argument:{value} is negative.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void PowerOfTwo(int value)
		{
			if (!MathUtils.IsPowerOfTwo(value))
			{
				throw new Exception($"{Library} Provided argument:{value} is not power of two.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TypeHasData<T>(World world, string suggestion)
		{
			if (!(world.SparseSet<T>() is DataSet<T>))
			{
				throw new Exception($"{Library} The type {typeof(T).GetFullGenericName()} has no associated data! " +
					$"{suggestion}, or enable {nameof(WorldConfig.StoreEmptyTypesAsDataSets)} in world config.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TypeHasData(SparseSet sparseSet, Type type, string suggestion)
		{
			if (!(sparseSet is IDataSet))
			{
				throw new Exception($"{Library} The type {type.GetFullGenericName()} has no associated data! {suggestion}, or enable {nameof(WorldConfig.StoreEmptyTypesAsDataSets)} in world config.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Has(SparseSet set, int id)
		{
			if (!set.Has(id))
			{
				throw new Exception($"{Library} The id:{id} is not added.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void HasPacked(SparseSet set, int index)
		{
			if (!set.HasPacked(index))
			{
				throw new Exception($"{Library} The index:{index} is invalid.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IsAlive(World world, int entityId)
		{
			IsAlive(world.Entities, entityId);
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IsAlive(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new Exception($"{Library} The entity with id:{entityId} is not alive.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IsAlive(World world, Entity entity)
		{
			IsAlive(world.Entities, entity);
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void IsAlive(Entities entities, Entity entity)
		{
			if (!entities.IsAlive(entity))
			{
				throw new Exception($"{Library} The {entity} is not alive.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CompatibleConfigs(World a, World b)
		{
			if (!a.Config.CompatibleWith(b.Config))
			{
				throw new Exception($"{Library} Worlds configs are incompatible.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CompatibleConfigs(SetFactory a, SetFactory b)
		{
			if (!a.CompatibleWith(b))
			{
				throw new Exception($"{Library} Worlds configs are incompatible.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void EqualPageSize(IPagedArray a, IPagedArray b)
		{
			if (a.PageSize != b.PageSize)
			{
				throw new Exception($"{Library} Paged arrays has different page sizes.");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void NoConflictsInFilter(SparseSet[] included, SparseSet[] excluded)
		{
			if (included.ContainsAny(excluded))
			{
				throw new Exception($"{Library} Conflicting included and excluded components!");
			}
		}

		[Conditional(Symbol)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ContainsDuplicates<T>(T[] array, string message) where T : class
		{
			for (var i = 0; i < array.Length; i++)
			{
				var set = array[i];
				for (var j = i + 1; j < array.Length; j++)
				{
					if (set == array[j])
					{
						throw new Exception(message);
					}
				}
			}
		}
	}
}

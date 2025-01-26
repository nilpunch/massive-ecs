#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Diagnostics;

namespace Massive
{
	public static class Assert
	{
		public const string Symbol = "MASSIVE_ASSERT";

		[Conditional(Symbol)]
		public static void That(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception(message);
			}
		}

		[Conditional(Symbol)]
		public static void TypeHasData<T>(Registry registry, string suggestion)
		{
			if (registry.Set<T>() is not DataSet<T>)
			{
				throw new Exception($"The type {typeof(T).GetFullGenericName()} has no associated data! {suggestion}, or enable {nameof(RegistryConfig.StoreEmptyTypesAsDataSets)} in registry config.");
			}
		}

		[Conditional(Symbol)]
		public static void IdAssigned(SparseSet set, int id)
		{
			if (!set.IsAssigned(id))
			{
				throw new Exception($"The id:{id} is not assigned.");
			}
		}

		[Conditional(Symbol)]
		public static void IdAssignedAt(SparseSet set, int index)
		{
			if (!set.IsAssignedAt(index))
			{
				throw new Exception($"The index:{index} is invalid.");
			}
		}

		[Conditional(Symbol)]
		public static void IsAlive(Registry registry, int entityId)
		{
			IsAlive(registry.Entities, entityId);
		}

		[Conditional(Symbol)]
		public static void IsAlive(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new Exception($"The entity with id:{entityId} is not alive.");
			}
		}

		[Conditional(Symbol)]
		public static void IsAlive(Registry registry, Entity entity)
		{
			if (!registry.IsAlive(entity))
			{
				throw new Exception($"The {entity} is not alive.");
			}
		}

		[Conditional(Symbol)]
		public static void NoConflictsInFilter(SparseSet[] included, SparseSet[] excluded)
		{
			if (included.ContainsAny(excluded))
			{
				throw new Exception("Conflicting included and excluded components!");
			}
		}

		[Conditional(Symbol)]
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

using System;
using System.Diagnostics;

namespace Massive
{
	public static class Debug
	{
		public const string Symbol = "MASSIVE_DEBUG";

		[Conditional(Symbol)]
		public static void Assert(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception(message);
			}
		}

		[Conditional(Symbol)]
		public static void AssertTypeHasData<T>(Registry registry, string suggestion)
		{
			if (registry.Set<T>() is not DataSet<T>)
			{
				throw new Exception($"The type {typeof(T).GetFullGenericName()} has no associated data! {suggestion}, or enable {nameof(RegistryConfig.StoreEmptyTypesAsDataSets)} in registry config.");
			}
		}

		[Conditional(Symbol)]
		public static void AssertIdAssigned(SparseSet set, int id)
		{
			if (!set.IsAssigned(id))
			{
				throw new Exception($"The id:{id} is not assigned.");
			}
		}

		[Conditional(Symbol)]
		public static void AssertIdAssignedAt(SparseSet set, int index)
		{
			if (!set.IsAssignedAt(index))
			{
				throw new Exception($"The index:{index} is invalid.");
			}
		}

		[Conditional(Symbol)]
		public static void AssertEntityAlive(Registry registry, int entityId)
		{
			AssertEntityAlive(registry.Entities, entityId);
		}

		[Conditional(Symbol)]
		public static void AssertEntityAlive(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new Exception($"The entity with id:{entityId} is not alive.");
			}
		}

		[Conditional(Symbol)]
		public static void AssertEntityAlive(Registry registry, Entity entity)
		{
			if (!registry.IsAlive(entity))
			{
				throw new Exception($"The {entity} is not alive.");
			}
		}

		[Conditional(Symbol)]
		public static void AssertNoConflicts(SparseSet[] included, SparseSet[] excluded)
		{
			if (included.ContainsAny(excluded))
			{
				throw new Exception("Conflicting included and excluded components!");
			}
		}

		[Conditional(Symbol)]
		public static void AssertContainsDuplicates<T>(T[] array, string message) where T : class
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

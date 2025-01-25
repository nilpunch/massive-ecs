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
		public static void AssertNotEmptyType<T>(Registry registry, string suggestion)
		{
			if (registry.Set<T>() is not DataSet<T>)
			{
				throw new Exception(ErrorMessage.TypeHasNoData<T>(suggestion));
			}
		}

		[Conditional(Symbol)]
		public static void AssertEntityAlive(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new Exception(ErrorMessage.EntityDead(entityId));
			}
		}

		[Conditional(Symbol)]
		public static void AssertEntityAlive(Registry registry, Entity entity)
		{
			if (!registry.IsAlive(entity))
			{
				throw new Exception(ErrorMessage.EntityDead(entity));
			}
		}

		[Conditional(Symbol)]
		public static void AssertEntityAlive(Registry registry, int entityId)
		{
			if (!registry.IsAlive(entityId))
			{
				throw new Exception(ErrorMessage.EntityDead(entityId));
			}
		}
	}
}

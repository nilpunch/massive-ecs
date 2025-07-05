using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidGetOperationException : MassiveException
	{
		private InvalidGetOperationException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfNotAdded(SparseSet sparseSet, int id)
		{
			if (!sparseSet.Has(id))
			{
				throw new InvalidGetOperationException($"You are trying to get data from the entity with id:{id} that is not added.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, Entity entity)
		{
			if (!entities.IsAlive(entity))
			{
				throw new InvalidGetOperationException($"You are trying to get a component from the dead entity {entity}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new InvalidGetOperationException($"You are trying to get a component from the dead entity with id:{entityId}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfLookupNotInitialized(int id, Sets sets, int typeIndex)
		{
			if (typeIndex >= sets.LookupCapacity || sets.Lookup[typeIndex] is null)
			{
				throw new InvalidGetOperationException($"You are trying to get data from the entity with id:{id} that is not added.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfLookupNotInitialized(Entity entity, Sets sets, int typeIndex)
		{
			if (typeIndex >= sets.LookupCapacity || sets.Lookup[typeIndex] is null)
			{
				throw new InvalidGetOperationException($"You are trying to get data from the entity {entity} that is not added.");
			}
		}
	}
}

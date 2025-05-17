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
				throw new InvalidGetOperationException($"You are trying to get data from the id:{id} that is not added.");
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
	}
}

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
		public static void ThrowIfNotAdded(BitSet bitSet, int id)
		{
			if (!bitSet.Has(id))
			{
				throw new InvalidGetOperationException($"You are trying to get data from the entity with id:{id} that is not added.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entifiers entifiers, Entifier entifier)
		{
			if (!entifiers.IsAlive(entifier))
			{
				throw new InvalidGetOperationException($"You are trying to get a component from the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entifiers entifiers, int entityId)
		{
			if (!entifiers.IsAlive(entityId))
			{
				throw new InvalidGetOperationException($"You are trying to get a component from the dead entity with id:{entityId}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfLookupNotInitialized(int id, BitSets bitSets, int typeIndex)
		{
			if (typeIndex >= bitSets.LookupCapacity || bitSets.Lookup[typeIndex] is null)
			{
				throw new InvalidGetOperationException($"You are trying to get data from the entity with id:{id} that is not added.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfLookupNotInitialized(Entifier entifier, BitSets bitSets, int typeIndex)
		{
			if (typeIndex >= bitSets.LookupCapacity || bitSets.Lookup[typeIndex] is null)
			{
				throw new InvalidGetOperationException($"You are trying to get data from the entity {entifier} that is not added.");
			}
		}
	}
}

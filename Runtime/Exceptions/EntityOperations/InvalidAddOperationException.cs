using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidAddOperationException : MassiveException
	{
		private InvalidAddOperationException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(in Entity entity)
		{
			if (!entity.IsAlive)
			{
				throw new InvalidAddOperationException($"You are trying to add a component to the dead entity {entity}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, Entifier entifier)
		{
			if (!entities.IsAlive(entifier))
			{
				throw new InvalidAddOperationException($"You are trying to add a component to the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new InvalidAddOperationException($"You are trying to add a component to the dead entity with id:{entityId}.");
			}
		}
	}
}

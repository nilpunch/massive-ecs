using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidRemoveOperationException : MassiveException
	{
		private InvalidRemoveOperationException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(in Entity entity)
		{
			if (!entity.IsAlive)
			{
				throw new InvalidRemoveOperationException($"You are trying to remove a component from the dead entity {entity}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, Entifier entifier)
		{
			if (!entities.IsAlive(entifier))
			{
				throw new InvalidRemoveOperationException($"You are trying to remove a component from the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new InvalidRemoveOperationException($"You are trying to remove a component from the dead entity with id:{entityId}.");
			}
		}
	}
}

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidSetOperationException : MassiveException
	{
		private InvalidSetOperationException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(in Entity entity)
		{
			if (!entity.IsAlive)
			{
				throw new InvalidSetOperationException($"You are trying to set a component to the dead entity {entity}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, Entifier entifier)
		{
			if (!entities.IsAlive(entifier))
			{
				throw new InvalidSetOperationException($"You are trying to set a component to the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new InvalidSetOperationException($"You are trying to set a component to the dead entity with id:{entityId}.");
			}
		}
	}
}

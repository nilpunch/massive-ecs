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
		public static void ThrowIfEntityDead(Entities entities, Entity entity)
		{
			if (!entities.IsAlive(entity))
			{
				throw new InvalidSetOperationException($"You are trying to set a component to the dead entity {entity}.");
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

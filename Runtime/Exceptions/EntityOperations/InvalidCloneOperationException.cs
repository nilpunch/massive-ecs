using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidCloneOperationException : MassiveException
	{
		private InvalidCloneOperationException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, Entity entity)
		{
			if (!entities.IsAlive(entity))
			{
				throw new InvalidCloneOperationException($"You are trying to clone the dead entity {entity}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new InvalidCloneOperationException($"You are trying to clone the dead entity with id:{entityId}.");
			}
		}
	}
}

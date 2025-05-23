using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class EntityNotAliveException : MassiveException
	{
		private EntityNotAliveException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, Entity entity)
		{
			if (!entities.IsAlive(entity))
			{
				throw new EntityNotAliveException($"You are trying to process the dead entity {entity}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new EntityNotAliveException($"You are trying to process the dead entity with id:{entityId}.");
			}
		}
	}
}

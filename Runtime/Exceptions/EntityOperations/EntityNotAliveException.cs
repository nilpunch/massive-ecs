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
		public static void ThrowIfEntityDead(Entifiers entifiers, Entifier entifier)
		{
			if (!entifiers.IsAlive(entifier))
			{
				throw new EntityNotAliveException($"You are trying to process the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entifiers entifiers, int entityId)
		{
			if (!entifiers.IsAlive(entityId))
			{
				throw new EntityNotAliveException($"You are trying to process the dead entity with id:{entityId}.");
			}
		}
	}
}

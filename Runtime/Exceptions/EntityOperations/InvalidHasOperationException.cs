using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidHasOperationException : MassiveException
	{
		private InvalidHasOperationException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entifiers entifiers, Entifier entifier)
		{
			if (!entifiers.IsAlive(entifier))
			{
				throw new InvalidHasOperationException($"You are trying to check a component on the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entifiers entifiers, int entityId)
		{
			if (!entifiers.IsAlive(entityId))
			{
				throw new InvalidHasOperationException($"You are trying to check a component on the dead entity with id:{entityId}.");
			}
		}
	}
}

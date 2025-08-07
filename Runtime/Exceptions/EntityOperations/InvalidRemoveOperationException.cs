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
		public static void ThrowIfEntityDead(Entifiers entifiers, Entifier entifier)
		{
			if (!entifiers.IsAlive(entifier))
			{
				throw new InvalidRemoveOperationException($"You are trying to remove a component from the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entifiers entifiers, int entityId)
		{
			if (!entifiers.IsAlive(entityId))
			{
				throw new InvalidRemoveOperationException($"You are trying to remove a component from the dead entity with id:{entityId}.");
			}
		}
	}
}

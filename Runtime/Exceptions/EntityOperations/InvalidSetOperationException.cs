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
		public static void ThrowIfEntityDead(Entifiers entifiers, Entifier entifier)
		{
			if (!entifiers.IsAlive(entifier))
			{
				throw new InvalidSetOperationException($"You are trying to set a component to the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entifiers entifiers, int entityId)
		{
			if (!entifiers.IsAlive(entityId))
			{
				throw new InvalidSetOperationException($"You are trying to set a component to the dead entity with id:{entityId}.");
			}
		}
	}
}

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
		public static void ThrowIfEntityDead(Entifiers entifiers, Entifier entifier)
		{
			if (!entifiers.IsAlive(entifier))
			{
				throw new InvalidAddOperationException($"You are trying to add a component to the dead entity {entifier}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entifiers entifiers, int entityId)
		{
			if (!entifiers.IsAlive(entityId))
			{
				throw new InvalidAddOperationException($"You are trying to add a component to the dead entity with id:{entityId}.");
			}
		}
	}
}

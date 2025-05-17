﻿using System.Diagnostics;
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
		public static void ThrowIfEntityDead(Entities entities, Entity entity)
		{
			if (!entities.IsAlive(entity))
			{
				throw new InvalidHasOperationException($"You are trying to check a component on the dead entity {entity}.");
			}
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfEntityDead(Entities entities, int entityId)
		{
			if (!entities.IsAlive(entityId))
			{
				throw new InvalidHasOperationException($"You are trying to check a component on the dead entity with id:{entityId}.");
			}
		}
	}
}

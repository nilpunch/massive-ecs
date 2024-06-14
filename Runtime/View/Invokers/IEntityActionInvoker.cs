﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityActionInvoker
	{
		void Apply(int id);
	}

	public struct EntityActionInvoker : IEntityActionInvoker
	{
		public EntityAction Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Action.Invoke(id);
		}
	}

	public struct EntityActionExtraInvoker<TExtra> : IEntityActionInvoker
	{
		public EntityActionExtra<TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Action.Invoke(id, Extra);
		}
	}

	public struct EntityFillInvoker : IEntityActionInvoker
	{
		public IList<int> Result;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Result.Add(id);
		}
	}
}
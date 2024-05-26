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
}

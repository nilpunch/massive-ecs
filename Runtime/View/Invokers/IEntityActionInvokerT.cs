using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityActionInvoker<T>
	{
		void Apply(int id, ref T a);
	}

	public struct EntityActionRefInvoker<T> : IEntityActionInvoker<T>
	{
		public EntityActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a);
		}
	}

	public struct EntityActionRefExtraInvoker<T, TExtra> : IEntityActionInvoker<T>
	{
		public EntityActionRefExtra<T, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a, Extra);
		}
	}
}

using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityActionInvoker<T1, T2, T3>
	{
		void Apply(int id, ref T1 a, ref T2 b, ref T3 c);
	}

	public struct EntityActionRefInvoker<T1, T2, T3> : IEntityActionInvoker<T1, T2, T3>
	{
		public EntityActionRef<T1, T2, T3> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(id, ref a, ref b, ref c);
		}
	}

	public struct EntityActionRefExtraInvoker<T1, T2, T3, TExtra> : IEntityActionInvoker<T1, T2, T3>
	{
		public EntityActionRefExtra<T1, T2, T3, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(id, ref a, ref b, ref c, Extra);
		}
	}
}

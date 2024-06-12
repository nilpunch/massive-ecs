using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityActionInvoker<T1, T2>
	{
		void Apply(int id, ref T1 a, ref T2 b);
	}

	public struct EntityActionRefInvoker<T1, T2> : IEntityActionInvoker<T1, T2>
	{
		public EntityActionRef<T1, T2> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(id, ref a, ref b);
		}
	}

	public struct EntityActionRefExtraInvoker<T1, T2, TExtra> : IEntityActionInvoker<T1, T2>
	{
		public EntityActionRefExtra<T1, T2, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(id, ref a, ref b, Extra);
		}
	}

	public struct ActionRefInvoker<T1, T2> : IEntityActionInvoker<T1, T2>
	{
		public ActionRef<T1, T2> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(ref a, ref b);
		}
	}

	public struct ActionRefExtraInvoker<T1, T2, TExtra> : IEntityActionInvoker<T1, T2>
	{
		public ActionRefExtra<T1, T2, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(ref a, ref b, Extra);
		}
	}
}

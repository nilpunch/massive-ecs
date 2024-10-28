using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T1, T2, T3, T4>
	{
		bool Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d);
	}

	public struct EntityActionRefAdapter<T1, T2, T3, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public EntityActionRef<T1, T2, T3, T4> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(id, ref a, ref b, ref c, ref d);
			return true;
		}
	}

	public struct EntityActionRefExtraAdapter<T1, T2, T3, T4, TExtra> : IEntityAction<T1, T2, T3, T4>
	{
		public EntityActionRefExtra<T1, T2, T3, T4, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(id, ref a, ref b, ref c, ref d, Extra);
			return true;
		}
	}

	public struct ActionRefAdapter<T1, T2, T3, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public ActionRef<T1, T2, T3, T4> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(ref a, ref b, ref c, ref d);
			return true;
		}
	}

	public struct ActionRefExtraAdapter<T1, T2, T3, TExtra, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public ActionRefExtra<T1, T2, T3, T4, TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(ref a, ref b, ref c, ref d, Extra);
			return true;
		}
	}
}

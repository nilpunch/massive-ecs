using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T1, T2, T3>
	{
		bool Apply(int id, ref T1 a, ref T2 b, ref T3 c);
	}

	public struct EntityActionRefAdapter<T1, T2, T3> : IEntityAction<T1, T2, T3>
	{
		public EntityActionRef<T1, T2, T3> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(id, ref a, ref b, ref c);
			return true;
		}
	}

	public struct EntityActionRefArgsAdapter<T1, T2, T3, TArgs> : IEntityAction<T1, T2, T3>
	{
		public EntityActionRefArgs<T1, T2, T3, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(id, ref a, ref b, ref c, Args);
			return true;
		}
	}

	public struct ActionRefAdapter<T1, T2, T3> : IEntityAction<T1, T2, T3>
	{
		public ActionRef<T1, T2, T3> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(ref a, ref b, ref c);
			return true;
		}
	}

	public struct ActionRefArgsAdapter<T1, T2, T3, TArgs> : IEntityAction<T1, T2, T3>
	{
		public ActionRefArgs<T1, T2, T3, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(ref a, ref b, ref c, Args);
			return true;
		}
	}
}

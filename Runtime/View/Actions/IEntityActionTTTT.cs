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

	public struct EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs> : IEntityAction<T1, T2, T3, T4>
	{
		public EntityActionRefArgs<T1, T2, T3, T4, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(id, ref a, ref b, ref c, ref d, Args);
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

	public struct ActionRefArgsAdapter<T1, T2, T3, TArgs, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public ActionRefArgs<T1, T2, T3, T4, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(ref a, ref b, ref c, ref d, Args);
			return true;
		}
	}
}

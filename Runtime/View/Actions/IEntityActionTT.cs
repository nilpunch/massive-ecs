using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T1, T2>
	{
		bool Apply(int id, ref T1 a, ref T2 b);
	}

	public struct EntityActionRefAdapter<T1, T2> : IEntityAction<T1, T2>
	{
		public EntityActionRef<T1, T2> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(id, ref a, ref b);
			return true;
		}
	}

	public struct EntityActionRefArgsAdapter<T1, T2, TArgs> : IEntityAction<T1, T2>
	{
		public EntityActionRefArgs<T1, T2, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(id, ref a, ref b, Args);
			return true;
		}
	}

	public struct ActionRefAdapter<T1, T2> : IEntityAction<T1, T2>
	{
		public ActionRef<T1, T2> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(ref a, ref b);
			return true;
		}
	}

	public struct ActionRefArgsAdapter<T1, T2, TArgs> : IEntityAction<T1, T2>
	{
		public ActionRefArgs<T1, T2, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(ref a, ref b, Args);
			return true;
		}
	}
}

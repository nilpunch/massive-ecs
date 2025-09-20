using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T1, T2, T3, T4>
	{
		void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d);
	}

	public struct IdActionRefAdapter<T1, T2, T3, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public IdActionRef<T1, T2, T3, T4> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(id, ref a, ref b, ref c, ref d);
		}
	}

	public struct IdActionRefArgsAdapter<T1, T2, T3, T4, TArgs> : IEntityAction<T1, T2, T3, T4>
	{
		public IdActionRefArgs<T1, T2, T3, T4, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(id, ref a, ref b, ref c, ref d, Args);
		}
	}

	public struct EntityActionRefAdapter<T1, T2, T3, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public EntityActionRef<T1, T2, T3, T4> Action;
		public Entities Entities;
		public World World;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(new Entity(id, Entities.Versions[id], World), ref a, ref b, ref c, ref d);
		}
	}

	public struct EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs> : IEntityAction<T1, T2, T3, T4>
	{
		public EntityActionRefArgs<T1, T2, T3, T4, TArgs> Action;
		public Entities Entities;
		public World World;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(new Entity(id, Entities.Versions[id], World), ref a, ref b, ref c, ref d, Args);
		}
	}

	public struct ActionRefAdapter<T1, T2, T3, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public ActionRef<T1, T2, T3, T4> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(ref a, ref b, ref c, ref d);
		}
	}

	public struct ActionRefArgsAdapter<T1, T2, T3, TArgs, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public ActionRefArgs<T1, T2, T3, T4, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(ref a, ref b, ref c, ref d, Args);
		}
	}
}

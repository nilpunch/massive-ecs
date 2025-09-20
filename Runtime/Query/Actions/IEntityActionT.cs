using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T>
	{
		void Apply(int id, ref T a);
	}

	public struct IdActionRefAdapter<T> : IEntityAction<T>
	{
		public IdActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a);
		}
	}

	public struct IdActionRefArgsAdapter<T, TArgs> : IEntityAction<T>
	{
		public IdActionRefArgs<T, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a, Args);
		}
	}

	public struct EntityActionRefAdapter<T> : IEntityAction<T>
	{
		public EntityActionRef<T> Action;
		public Entities Entities;
		public World World;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(new Entity(id, Entities.Versions[id], World), ref a);
		}
	}

	public struct EntityActionRefArgsAdapter<T, TArgs> : IEntityAction<T>
	{
		public EntityActionRefArgs<T, TArgs> Action;
		public Entities Entities;
		public World World;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(new Entity(id, Entities.Versions[id], World), ref a, Args);
		}
	}

	public struct ActionRefAdapter<T> : IEntityAction<T>
	{
		public ActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(ref a);
		}
	}

	public struct ActionRefArgsAdapter<T, TArgs> : IEntityAction<T>
	{
		public ActionRefArgs<T, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(ref a, Args);
		}
	}
}

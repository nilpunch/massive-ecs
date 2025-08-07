using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T>
	{
		bool Apply(int id, ref T a);
	}

	public struct IdActionRefAdapter<T> : IEntityAction<T>
	{
		public IdActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a);
			return true;
		}
	}

	public struct IdActionRefArgsAdapter<T, TArgs> : IEntityAction<T>
	{
		public IdActionRefArgs<T, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a, Args);
			return true;
		}
	}

	public struct EntityActionRefAdapter<T> : IEntityAction<T>
	{
		public EntityActionRef<T> Action;
		public Entifiers Entifiers;
		public World World;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(new Entity(id, Entifiers.Versions[id], World), ref a);
			return true;
		}
	}

	public struct EntityActionRefArgsAdapter<T, TArgs> : IEntityAction<T>
	{
		public EntityActionRefArgs<T, TArgs> Action;
		public Entifiers Entifiers;
		public World World;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(new Entity(id, Entifiers.Versions[id], World), ref a, Args);
			return true;
		}
	}

	public struct ActionRefAdapter<T> : IEntityAction<T>
	{
		public ActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(ref a);
			return true;
		}
	}

	public struct ActionRefArgsAdapter<T, TArgs> : IEntityAction<T>
	{
		public ActionRefArgs<T, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id, ref T a)
		{
			Action.Invoke(ref a, Args);
			return true;
		}
	}
}

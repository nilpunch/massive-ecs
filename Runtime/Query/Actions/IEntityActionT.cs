using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T>
	{
		void Apply(int id, ref T a);
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdActionRefAdapter<T> : IEntityAction<T>
	{
		public IdActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(id, ref a);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
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

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionRefAdapter<T> : IEntityAction<T>
	{
		public EntityActionRef<T> Action;
		public Entities Entities;
		public Entity Entity;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Entity.Id = id;
			Entity.Version = Entities.Versions[id];
			Action.Invoke(Entity, ref a);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionRefArgsAdapter<T, TArgs> : IEntityAction<T>
	{
		public EntityActionRefArgs<T, TArgs> Action;
		public Entities Entities;
		public Entity Entity;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Entity.Id = id;
			Entity.Version = Entities.Versions[id];
			Action.Invoke(Entity, ref a, Args);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ActionRefAdapter<T> : IEntityAction<T>
	{
		public ActionRef<T> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T a)
		{
			Action.Invoke(ref a);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
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

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T1, T2>
	{
		void Apply(int id, ref T1 a, ref T2 b);
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdActionRefAdapter<T1, T2> : IEntityAction<T1, T2>
	{
		public IdActionRef<T1, T2> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(id, ref a, ref b);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdActionRefArgsAdapter<T1, T2, TArgs> : IEntityAction<T1, T2>
	{
		public IdActionRefArgs<T1, T2, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(id, ref a, ref b, Args);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionRefAdapter<T1, T2> : IEntityAction<T1, T2>
	{
		public EntityActionRef<T1, T2> Action;
		public Entities Entities;
		public Entity Entity;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Entity.Id = id;
			Entity.Version = Entities.Versions[id];
			Action.Invoke(Entity, ref a, ref b);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionRefArgsAdapter<T1, T2, TArgs> : IEntityAction<T1, T2>
	{
		public EntityActionRefArgs<T1, T2, TArgs> Action;
		public Entities Entities;
		public Entity Entity;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Entity.Id = id;
			Entity.Version = Entities.Versions[id];
			Action.Invoke(Entity, ref a, ref b, Args);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ActionRefAdapter<T1, T2> : IEntityAction<T1, T2>
	{
		public ActionRef<T1, T2> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(ref a, ref b);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ActionRefArgsAdapter<T1, T2, TArgs> : IEntityAction<T1, T2>
	{
		public ActionRefArgs<T1, T2, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b)
		{
			Action.Invoke(ref a, ref b, Args);
		}
	}
}

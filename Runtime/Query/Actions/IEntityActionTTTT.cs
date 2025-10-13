using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T1, T2, T3, T4>
	{
		void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d);
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdActionRefAdapter<T1, T2, T3, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public IdActionRef<T1, T2, T3, T4> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(id, ref a, ref b, ref c, ref d);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
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

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionRefAdapter<T1, T2, T3, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public EntityActionRef<T1, T2, T3, T4> Action;
		public Entities Entities;
		public Entity Entity;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Entity.Id = id;
			Entity.Version = Entities.Versions[id];
			Action.Invoke(Entity, ref a, ref b, ref c, ref d);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionRefArgsAdapter<T1, T2, T3, T4, TArgs> : IEntityAction<T1, T2, T3, T4>
	{
		public EntityActionRefArgs<T1, T2, T3, T4, TArgs> Action;
		public Entities Entities;
		public Entity Entity;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Entity.Id = id;
			Entity.Version = Entities.Versions[id];
			Action.Invoke(Entity, ref a, ref b, ref c, ref d, Args);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ActionRefAdapter<T1, T2, T3, T4> : IEntityAction<T1, T2, T3, T4>
	{
		public ActionRef<T1, T2, T3, T4> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c, ref T4 d)
		{
			Action.Invoke(ref a, ref b, ref c, ref d);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ActionRefArgsAdapter<T1, T2, T3, T4, TArgs> : IEntityAction<T1, T2, T3, T4>
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

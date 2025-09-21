﻿using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public interface IEntityAction<T1, T2, T3>
	{
		void Apply(int id, ref T1 a, ref T2 b, ref T3 c);
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdActionRefAdapter<T1, T2, T3> : IEntityAction<T1, T2, T3>
	{
		public IdActionRef<T1, T2, T3> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(id, ref a, ref b, ref c);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdActionRefArgsAdapter<T1, T2, T3, TArgs> : IEntityAction<T1, T2, T3>
	{
		public IdActionRefArgs<T1, T2, T3, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(id, ref a, ref b, ref c, Args);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionRefAdapter<T1, T2, T3> : IEntityAction<T1, T2, T3>
	{
		public EntityActionRef<T1, T2, T3> Action;
		public Entities Entities;
		public World World;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(new Entity(id, Entities.Versions[id], World), ref a, ref b, ref c);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionRefArgsAdapter<T1, T2, T3, TArgs> : IEntityAction<T1, T2, T3>
	{
		public EntityActionRefArgs<T1, T2, T3, TArgs> Action;
		public Entities Entities;
		public World World;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(new Entity(id, Entities.Versions[id], World), ref a, ref b, ref c, Args);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ActionRefAdapter<T1, T2, T3> : IEntityAction<T1, T2, T3>
	{
		public ActionRef<T1, T2, T3> Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(ref a, ref b, ref c);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct ActionRefArgsAdapter<T1, T2, T3, TArgs> : IEntityAction<T1, T2, T3>
	{
		public ActionRefArgs<T1, T2, T3, TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id, ref T1 a, ref T2 b, ref T3 c)
		{
			Action.Invoke(ref a, ref b, ref c, Args);
		}
	}
}

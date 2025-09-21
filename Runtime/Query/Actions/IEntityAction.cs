using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	public interface IEntityAction
	{
		void Apply(int id);
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdActionAdapter : IEntityAction
	{
		public IdAction Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Action.Invoke(id);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct IdActionArgsAdapter<TArgs> : IEntityAction
	{
		public IdActionArgs<TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Action.Invoke(id, Args);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionAdapter : IEntityAction
	{
		public EntityAction Action;
		public Entities Entities;
		public World World;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Action.Invoke(new Entity(id, Entities.Versions[id], World));
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct EntityActionArgsAdapter<TArgs> : IEntityAction
	{
		public EntityActionArgs<TArgs> Action;
		public Entities Entities;
		public World World;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Action.Invoke(new Entity(id, Entities.Versions[id], World), Args);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct FillIds : IEntityAction
	{
		public IList<int> Result;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Result.Add(id);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct FillEntifiers : IEntityAction
	{
		public IList<Entifier> Result;
		public Entities Entities;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Result.Add(new Entifier(id, Entities.Versions[id]));
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct DestroyAll : IEntityAction
	{
		public Entities Entities;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Entities.Destroy(id);
		}
	}

	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct CountAll : IEntityAction
	{
		public int Result;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Result += 1;
		}
	}
}

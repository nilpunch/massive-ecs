using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction
	{
		bool Apply(int id);
	}

	public struct EntityActionAdapter : IEntityAction
	{
		public EntityAction Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id)
		{
			Action.Invoke(id);
			return true;
		}
	}

	public struct EntityActionArgsAdapter<TArgs> : IEntityAction
	{
		public EntityActionArgs<TArgs> Action;
		public TArgs Args;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id)
		{
			Action.Invoke(id, Args);
			return true;
		}
	}

	public struct FillIds : IEntityAction
	{
		public IList<int> Result;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id)
		{
			Result.Add(id);
			return true;
		}
	}

	public struct FillEntities : IEntityAction
	{
		public IList<Entity> Result;
		public Entities Entities;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id)
		{
			Result.Add(Entities.GetEntity(id));
			return true;
		}
	}

	public struct ReturnFirst : IEntityAction
	{
		public int Result;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id)
		{
			Result = id;
			return false;
		}
	}

	public struct DestroyAll : IEntityAction
	{
		public Entities Entities;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id)
		{
			Entities.Destroy(id);
			return true;
		}
	}

	public struct CountAll : IEntityAction
	{
		public int Result;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Apply(int id)
		{
			Result += 1;
			return true;
		}
	}
}

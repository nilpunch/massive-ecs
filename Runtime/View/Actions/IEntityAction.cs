using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public interface IEntityAction
	{
		void Apply(int id);
	}

	public struct EntityActionAdapter : IEntityAction
	{
		public EntityAction Action;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Action.Invoke(id);
		}
	}

	public struct EntityActionExtraAdapter<TExtra> : IEntityAction
	{
		public EntityActionExtra<TExtra> Action;
		public TExtra Extra;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Action.Invoke(id, Extra);
		}
	}

	public struct FillIds : IEntityAction
	{
		public IList<int> Result;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Result.Add(id);
		}
	}

	public struct FillEntities : IEntityAction
	{
		public IList<Entity> Result;
		public Entities Entities;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Apply(int id)
		{
			Result.Add(Entities.GetEntity(id));
		}
	}
}

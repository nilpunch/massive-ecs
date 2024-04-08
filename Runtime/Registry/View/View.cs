using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View
	{
		private readonly Entities _entities;

		public View(IRegistry registry)
		{
			_entities = registry.Entities;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			var entities = _entities.Alive;
			for (var i = entities.Length - 1; i >= 0; i--)
			{
				action.Invoke(entities[i].Id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionExtra<TExtra> action)
		{
			var entities = _entities.Alive;
			for (var i = entities.Length - 1; i >= 0; i--)
			{
				action.Invoke(entities[i].Id, extra);
			}
		}
	}
}
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View
	{
		private readonly Identifiers _entities;

		public View(IRegistry registry)
		{
			_entities = registry.Entities;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			var ids = _entities.AliveIds;
			for (var i = ids.Length - 1; i >= 0; i--)
			{
				action.Invoke(ids[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionExtra<TExtra> action)
		{
			var ids = _entities.AliveIds;
			for (var i = ids.Length - 1; i >= 0; i--)
			{
				action.Invoke(ids[i], extra);
			}
		}
	}
}
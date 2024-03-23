using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct FilterView
	{
		private readonly Identifiers _entities;
		private readonly ISet[] _include;
		private readonly ISet[] _exclude;

		public FilterView(IRegistry registry, ISet[] include = null, ISet[] exclude = null)
		{
			include ??= Array.Empty<ISet>();
			exclude ??= Array.Empty<ISet>();

			_entities = registry.Entities;
			_include = include;
			_exclude = exclude;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityAction action)
		{
			var ids = _include.Length == 0
				? _entities.AliveIds
				: ViewUtils.GetMinimalSet(_include).AliveIds;

			for (var i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				if (ViewUtils.AliveInAll(id, _include) && ViewUtils.NotAliveInAll(id, _exclude))
				{
					action.Invoke(id);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionExtra<TExtra> action)
		{
			var ids = _include.Length == 0
				? _entities.AliveIds
				: ViewUtils.GetMinimalSet(_include).AliveIds;

			for (var i = ids.Length - 1; i >= 0; i--)
			{
				var id = ids[i];
				if (ViewUtils.AliveInAll(id, _include) && ViewUtils.NotAliveInAll(id, _exclude))
				{
					action.Invoke(id, extra);
				}
			}
		}
	}
}
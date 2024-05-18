using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView<T>
	{
		private readonly IGroup _group;
		private readonly IReadOnlyDataSet<T> _components;

		public GroupView(IRegistry registry, IGroup group)
		{
			_group = group;
			_components = registry.Components<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			_group.EnsureSynced();

			var data = _components.Data;
			var groupIds = _group.Ids;

			if (_group.IsOwning(_components))
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, groupIds.Length))
				{
					var page = data.Pages[pageIndex];
					for (int dense = pageLength - 1; dense >= 0; dense--)
					{
						int id = groupIds[indexOffset + dense];
						action.Invoke(id, ref page[dense]);
					}
				}
			}
			else
			{
				for (int dense = groupIds.Length - 1; dense >= 0; dense--)
				{
					int id = groupIds[dense];
					action.Invoke(id, ref _components.Get(id));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T, TExtra> action)
		{
			_group.EnsureSynced();

			var data = _components.Data;
			var groupIds = _group.Ids;

			if (_group.IsOwning(_components))
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, groupIds.Length))
				{
					var page = data.Pages[pageIndex];
					for (int dense = pageLength - 1; dense >= 0; dense--)
					{
						int id = groupIds[indexOffset + dense];
						action.Invoke(id, ref page[dense], extra);
					}
				}
			}
			else
			{
				for (int dense = groupIds.Length - 1; dense >= 0; dense--)
				{
					int id = groupIds[dense];
					action.Invoke(id, ref _components.Get(id), extra);
				}
			}
		}
	}
}

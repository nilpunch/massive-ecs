using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView<T> : IView<T>
	{
		private readonly IGroup _group;
		private readonly IReadOnlyDataSet<T> _components;

		public GroupView(IRegistry registry, IGroup group)
		{
			_group = group;
			_components = registry.Components<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T>
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
						invoker.Apply(id, ref page[dense]);
					}
				}
			}
			else
			{
				for (int dense = groupIds.Length - 1; dense >= 0; dense--)
				{
					int id = groupIds[dense];
					invoker.Apply(id, ref _components.Get(id));
				}
			}
		}
	}
}

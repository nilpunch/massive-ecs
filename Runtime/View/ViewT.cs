using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View<T>
	{
		private readonly IReadOnlyDataSet<T> _components;

		public View(IRegistry registry)
		{
			_components = registry.Components<T>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T> action)
		{
			var data = _components.Data;
			var ids = _components.Ids;
			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, _components.Count))
			{
				var page = data.Pages[pageIndex];
				for (int dense = pageLength - 1; dense >= 0; dense--)
				{
					var id = ids[indexOffset + dense];
					action.Invoke(id, ref page[dense]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T, TExtra> action)
		{
			var data = _components.Data;
			var ids = _components.Ids;
			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, _components.Count))
			{
				var page = data.Pages[pageIndex];
				for (int dense = pageLength - 1; dense >= 0; dense--)
				{
					var id = ids[indexOffset + dense];
					action.Invoke(id, ref page[dense], extra);
				}
			}
		}
	}
}

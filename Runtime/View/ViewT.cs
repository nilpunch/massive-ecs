using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View<T> : IView<T>
	{
		private readonly IReadOnlyDataSet<T> _components;

		public View(IReadOnlyDataSet<T> components)
		{
			_components = components;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachUniversal<TInvoker>(TInvoker invoker)
			where TInvoker : IEntityActionInvoker<T>
		{
			var data = _components.Data;
			var ids = _components.Ids;
			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, _components.Count))
			{
				var page = data.Pages[pageIndex];
				for (int dense = pageLength - 1; dense >= 0; dense--)
				{
					var id = ids[indexOffset + dense];
					invoker.Apply(id, ref page[dense]);
				}
			}
		}
	}
}

using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View<T> where T : struct
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
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				action.Invoke(ids[dense], ref data[dense]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T, TExtra> action)
		{
			var data = _components.Data;
			var ids = _components.Ids;
			for (int dense = ids.Length - 1; dense >= 0; dense--)
			{
				action.Invoke(ids[dense], ref data[dense], extra);
			}
		}
	}
}
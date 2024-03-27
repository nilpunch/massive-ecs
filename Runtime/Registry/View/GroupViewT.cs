using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView<T> where T : struct
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
			var data = _components.AliveData;
			var groupIds = _group.GroupIds;

			if (_group.IsOwning(_components))
			{
				for (int dense = groupIds.Length - 1; dense >= 0; dense--)
				{
					action.Invoke(groupIds[dense], ref data[dense]);
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
			var data = _components.AliveData;
			var groupIds = _group.GroupIds;

			if (_group.IsOwning(_components))
			{
				for (int dense = groupIds.Length - 1; dense >= 0; dense--)
				{
					action.Invoke(groupIds[dense], ref data[dense], extra);
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
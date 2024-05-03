using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView<T1, T2>
	{
		private readonly IGroup _group;
		private readonly IReadOnlyDataSet<T1> _components1;
		private readonly IReadOnlyDataSet<T2> _components2;

		public GroupView(IRegistry registry, IGroup group)
		{
			_group = group;
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2> action)
		{
			_group.EnsureSynced();

			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var groupIds = _group.Ids;

			switch (_group.IsOwning(_components1), _group.IsOwning(_components2))
			{
				case (true, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						action.Invoke(groupIds[dense], ref data1[dense], ref data2[dense]);
					}
					break;

				case (false, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref data2[dense]);
					}
					break;

				case (true, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref _components2.Get(id));
					}
					break;

				case (false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref _components2.Get(id));
					}
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T1, T2, TExtra> action)
		{
			_group.EnsureSynced();

			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var groupIds = _group.Ids;

			switch (_group.IsOwning(_components1), _group.IsOwning(_components2))
			{
				case (true, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						action.Invoke(groupIds[dense], ref data1[dense], ref data2[dense], extra);
					}
					break;

				case (false, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref data2[dense], extra);
					}
					break;

				case (true, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref _components2.Get(id), extra);
					}
					break;

				case (false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref _components2.Get(id), extra);
					}
					break;
			}
		}
	}
}

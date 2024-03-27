using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView<T1, T2, T3>
		where T1 : struct
		where T2 : struct
		where T3 : struct
	{
		private readonly IGroup _group;
		private readonly IReadOnlyDataSet<T1> _components1;
		private readonly IReadOnlyDataSet<T2> _components2;
		private readonly IReadOnlyDataSet<T3> _components3;

		public GroupView(IRegistry registry, IGroup group)
		{
			_group = group;
			_components1 = registry.Components<T1>();
			_components2 = registry.Components<T2>();
			_components3 = registry.Components<T3>();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach(EntityActionRef<T1, T2, T3> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var data3 = _components3.AliveData;
			var groupIds = _group.GroupIds;

			switch (_group.IsOwning(_components1), _group.IsOwning(_components2), _group.IsOwning(_components3))
			{
				case (true, true, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref data2[dense], ref data3[dense]);
					}
					break;

				case (false, true, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref data2[dense], ref data3[dense]);
					}
					break;

				case (true, false, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref _components2.Get(id), ref data3[dense]);
					}
					break;

				case (false, false, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref _components2.Get(id), ref data3[dense]);
					}
					break;

				case (true, true, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref data2[dense], ref _components3.Get(id));
					}
					break;

				case (false, true, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref data2[dense], ref _components3.Get(id));
					}
					break;

				case (true, false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref _components2.Get(id), ref _components3.Get(id));
					}
					break;

				case (false, false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref _components2.Get(id), ref _components3.Get(id));
					}
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEachExtra<TExtra>(TExtra extra, EntityActionRefExtra<T1, T2, T3, TExtra> action)
		{
			var data1 = _components1.AliveData;
			var data2 = _components2.AliveData;
			var data3 = _components3.AliveData;
			var groupIds = _group.GroupIds;

			switch (_group.IsOwning(_components1), _group.IsOwning(_components2), _group.IsOwning(_components3))
			{
				case (true, true, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref data2[dense], ref data3[dense], extra);
					}
					break;

				case (false, true, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref data2[dense], ref data3[dense], extra);
					}
					break;

				case (true, false, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref _components2.Get(id), ref data3[dense], extra);
					}
					break;

				case (false, false, true):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref _components2.Get(id), ref data3[dense], extra);
					}
					break;

				case (true, true, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref data2[dense], ref _components3.Get(id), extra);
					}
					break;

				case (false, true, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref data2[dense], ref _components3.Get(id), extra);
					}
					break;

				case (true, false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref data1[dense], ref _components2.Get(id), ref _components3.Get(id), extra);
					}
					break;

				case (false, false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Invoke(id, ref _components1.Get(id), ref _components2.Get(id), ref _components3.Get(id), extra);
					}
					break;
			}
		}
	}
}
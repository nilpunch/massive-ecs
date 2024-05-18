using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView<T1, T2, T3>
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
			_group.EnsureSynced();

			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var data3 = _components3.Data;
			var groupIds = _group.Ids;

			switch (_group.IsOwning(_components1), _group.IsOwning(_components2), _group.IsOwning(_components3))
			{
				case (true, true, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						var page2 = data2.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref page1[dense], ref page2[dense], ref page3[dense]);
						}
					}
					break;

				case (false, true, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page2 = data2.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref _components1.Get(id), ref page2[dense], ref page3[dense]);
						}
					}
					break;

				case (true, false, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref page1[dense], ref _components2.Get(id), ref page3[dense]);
						}
					}
					break;

				case (false, false, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page3 = data3.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref _components1.Get(id), ref _components2.Get(id), ref page3[dense]);
						}
					}
					break;

				case (true, true, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						var page2 = data2.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref page1[dense], ref page2[dense], ref _components3.Get(id));
						}
					}
					break;

				case (false, true, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page2 = data2.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref _components1.Get(id), ref page2[dense], ref _components3.Get(id));
						}
					}
					break;

				case (true, false, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref page1[dense], ref _components2.Get(id), ref _components3.Get(id));
						}
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
			_group.EnsureSynced();

			_group.EnsureSynced();

			var data1 = _components1.Data;
			var data2 = _components2.Data;
			var data3 = _components3.Data;
			var groupIds = _group.Ids;

			switch (_group.IsOwning(_components1), _group.IsOwning(_components2), _group.IsOwning(_components3))
			{
				case (true, true, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						var page2 = data2.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref page1[dense], ref page2[dense], ref page3[dense], extra);
						}
					}
					break;

				case (false, true, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page2 = data2.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref _components1.Get(id), ref page2[dense], ref page3[dense], extra);
						}
					}
					break;

				case (true, false, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref page1[dense], ref _components2.Get(id), ref page3[dense], extra);
						}
					}
					break;

				case (false, false, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page3 = data3.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref _components1.Get(id), ref _components2.Get(id), ref page3[dense], extra);
						}
					}
					break;

				case (true, true, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						var page2 = data2.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref page1[dense], ref page2[dense], ref _components3.Get(id), extra);
						}
					}
					break;

				case (false, true, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page2 = data2.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref _components1.Get(id), ref page2[dense], ref _components3.Get(id), extra);
						}
					}
					break;

				case (true, false, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Invoke(id, ref page1[dense], ref _components2.Get(id), ref _components3.Get(id), extra);
						}
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

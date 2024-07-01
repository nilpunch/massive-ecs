using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView : IView
	{
		private IGroup Group { get; }

		public IRegistry Registry { get; }

		public GroupView(IRegistry registry, IGroup group)
		{
			Registry = registry;
			Group = group;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction>(TAction action)
			where TAction : IEntityAction
		{
			Group.EnsureSynced();

			var groupIds = Group.Ids;
			for (var i = groupIds.Length - 1; i >= 0; i--)
			{
				action.Apply(groupIds[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction, T>(TAction action)
			where TAction : IEntityAction<T>
		{
			Group.EnsureSynced();

			var components = Registry.Components<T>();

			var data = components.Data;
			var groupIds = Group.Ids;

			if (Group.IsOwning(components))
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, groupIds.Length))
				{
					var page = data.Pages[pageIndex];
					for (int dense = pageLength - 1; dense >= 0; dense--)
					{
						int id = groupIds[indexOffset + dense];
						action.Apply(id, ref page[dense]);
					}
				}
			}
			else
			{
				for (int dense = groupIds.Length - 1; dense >= 0; dense--)
				{
					int id = groupIds[dense];
					action.Apply(id, ref components.Get(id));
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction, T1, T2>(TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			Group.EnsureSynced();

			var components1 = Registry.Components<T1>();
			var components2 = Registry.Components<T2>();

			var data1 = components1.Data;
			var data2 = components2.Data;
			var groupIds = Group.Ids;

			switch (Group.IsOwning(components1), Group.IsOwning(components2))
			{
				case (true, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						var page2 = data2.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Apply(id, ref page1[dense], ref page2[dense]);
						}
					}
					break;

				case (false, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page2 = data2.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Apply(id, ref components1.Get(id), ref page2[dense]);
						}
					}
					break;

				case (true, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, groupIds.Length))
					{
						var page1 = data1.Pages[pageIndex];
						for (int dense = pageLength - 1; dense >= 0; dense--)
						{
							int id = groupIds[indexOffset + dense];
							action.Apply(id, ref page1[dense], ref components2.Get(id));
						}
					}
					break;

				case (false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Apply(id, ref components1.Get(id), ref components2.Get(id));
					}
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction, T1, T2, T3>(TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			Group.EnsureSynced();

			var components1 = Registry.Components<T1>();
			var components2 = Registry.Components<T2>();
			var components3 = Registry.Components<T3>();

			var data1 = components1.Data;
			var data2 = components2.Data;
			var data3 = components3.Data;
			var groupIds = Group.Ids;

			switch (Group.IsOwning(components1), Group.IsOwning(components2), Group.IsOwning(components3))
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
							action.Apply(id, ref page1[dense], ref page2[dense], ref page3[dense]);
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
							action.Apply(id, ref components1.Get(id), ref page2[dense], ref page3[dense]);
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
							action.Apply(id, ref page1[dense], ref components2.Get(id), ref page3[dense]);
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
							action.Apply(id, ref components1.Get(id), ref components2.Get(id), ref page3[dense]);
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
							action.Apply(id, ref page1[dense], ref page2[dense], ref components3.Get(id));
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
							action.Apply(id, ref components1.Get(id), ref page2[dense], ref components3.Get(id));
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
							action.Apply(id, ref page1[dense], ref components2.Get(id), ref components3.Get(id));
						}
					}
					break;

				case (false, false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Apply(id, ref components1.Get(id), ref components2.Get(id), ref components3.Get(id));
					}
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ReadOnlySpan<int>.Enumerator GetEnumerator()
		{
			return Group.Ids.GetEnumerator();
		}
	}
}

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

		public void ForEach<TAction, T>(TAction action)
			where TAction : IEntityAction<T>
		{
			Group.EnsureSynced();

			var dataSet = Registry.DataSet<T>();

			var data = dataSet.Data;
			var groupIds = Group.Ids;

			if (Group.IsOwning(dataSet))
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
					action.Apply(id, ref dataSet.Get(id));
				}
			}
		}

		public void ForEach<TAction, T1, T2>(TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			Group.EnsureSynced();

			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var groupIds = Group.Ids;

			switch (Group.IsOwning(dataSet1), Group.IsOwning(dataSet2))
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
							action.Apply(id, ref dataSet1.Get(id), ref page2[dense]);
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
							action.Apply(id, ref page1[dense], ref dataSet2.Get(id));
						}
					}
					break;

				case (false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Apply(id, ref dataSet1.Get(id), ref dataSet2.Get(id));
					}
					break;
			}
		}

		public void ForEach<TAction, T1, T2, T3>(TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			Group.EnsureSynced();

			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var groupIds = Group.Ids;

			switch (Group.IsOwning(dataSet1), Group.IsOwning(dataSet2), Group.IsOwning(dataSet3))
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
							action.Apply(id, ref dataSet1.Get(id), ref page2[dense], ref page3[dense]);
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
							action.Apply(id, ref page1[dense], ref dataSet2.Get(id), ref page3[dense]);
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
							action.Apply(id, ref dataSet1.Get(id), ref dataSet2.Get(id), ref page3[dense]);
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
							action.Apply(id, ref page1[dense], ref page2[dense], ref dataSet3.Get(id));
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
							action.Apply(id, ref dataSet1.Get(id), ref page2[dense], ref dataSet3.Get(id));
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
							action.Apply(id, ref page1[dense], ref dataSet2.Get(id), ref dataSet3.Get(id));
						}
					}
					break;

				case (false, false, false):
					for (int dense = groupIds.Length - 1; dense >= 0; dense--)
					{
						int id = groupIds[dense];
						action.Apply(id, ref dataSet1.Get(id), ref dataSet2.Get(id), ref dataSet3.Get(id));
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

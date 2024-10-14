using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct GroupView : IView
	{
		private IGroup Group { get; }

		public Registry Registry { get; }

		public GroupView(Registry registry, IGroup group)
		{
			Registry = registry;
			Group = group;
		}

		public void ForEach<TAction>(TAction action)
			where TAction : IEntityAction
		{
			Group.EnsureSynced();

			var groupSet = Group.Set;
			for (var i = Group.Count - 1; i >= 0; i--)
			{
				if (i > Group.Count)
				{
					i = Group.Count;
					continue;
				}

				action.Apply(groupSet.Ids[i]);
			}
		}

		public void ForEach<TAction, T>(TAction action)
			where TAction : IEntityAction<T>
		{
			Group.EnsureSynced();

			var dataSet = Registry.DataSet<T>();

			var data = dataSet.Data;
			var groupSet = Group.Set;

			if (Group.IsOwning(dataSet))
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, Group.Count))
				{
					var page = data.Pages[pageIndex];
					for (int packed = pageLength - 1; packed >= 0; packed--)
					{
						if (indexOffset + packed > Group.Count)
						{
							packed = Group.Count - indexOffset;
							continue;
						}

						int id = groupSet.Ids[indexOffset + packed];
						action.Apply(id, ref page[packed]);
					}
				}
			}
			else
			{
				for (int packed = Group.Count - 1; packed >= 0; packed--)
				{
					if (packed > Group.Count)
					{
						packed = Group.Count;
						continue;
					}

					int id = groupSet.Ids[packed];
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
			var groupSet = Group.Set;

			switch (Group.IsOwning(dataSet1), Group.IsOwning(dataSet2))
			{
				case (true, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page1 = data1.Pages[pageIndex];
						var page2 = data2.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref page1[packed], ref page2[packed]);
						}
					}
					break;

				case (false, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page2 = data2.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref dataSet1.Get(id), ref page2[packed]);
						}
					}
					break;

				case (true, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page1 = data1.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref page1[packed], ref dataSet2.Get(id));
						}
					}
					break;

				case (false, false):
					for (int packed = Group.Count - 1; packed >= 0; packed--)
					{
						if (packed > Group.Count)
						{
							packed = Group.Count;
							continue;
						}

						int id = groupSet.Ids[packed];
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
			var groupSet = Group.Set;

			switch (Group.IsOwning(dataSet1), Group.IsOwning(dataSet2), Group.IsOwning(dataSet3))
			{
				case (true, true, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page1 = data1.Pages[pageIndex];
						var page2 = data2.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref page1[packed], ref page2[packed], ref page3[packed]);
						}
					}
					break;

				case (false, true, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page2 = data2.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref dataSet1.Get(id), ref page2[packed], ref page3[packed]);
						}
					}
					break;

				case (true, false, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page1 = data1.Pages[pageIndex];
						var page3 = data3.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref page1[packed], ref dataSet2.Get(id), ref page3[packed]);
						}
					}
					break;

				case (false, false, true):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page3 = data3.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref dataSet1.Get(id), ref dataSet2.Get(id), ref page3[packed]);
						}
					}
					break;

				case (true, true, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page1 = data1.Pages[pageIndex];
						var page2 = data2.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref page1[packed], ref page2[packed], ref dataSet3.Get(id));
						}
					}
					break;

				case (false, true, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page2 = data2.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref dataSet1.Get(id), ref page2[packed], ref dataSet3.Get(id));
						}
					}
					break;

				case (true, false, false):
					foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
					{
						var page1 = data1.Pages[pageIndex];
						for (int packed = pageLength - 1; packed >= 0; packed--)
						{
							if (indexOffset + packed > Group.Count)
							{
								packed = Group.Count - indexOffset;
								continue;
							}

							int id = groupSet.Ids[indexOffset + packed];
							action.Apply(id, ref page1[packed], ref dataSet2.Get(id), ref dataSet3.Get(id));
						}
					}
					break;

				case (false, false, false):
					for (int packed = Group.Count - 1; packed >= 0; packed--)
					{
						if (packed > Group.Count)
						{
							packed = Group.Count;
							continue;
						}

						int id = groupSet.Ids[packed];
						action.Apply(id, ref dataSet1.Get(id), ref dataSet2.Get(id), ref dataSet3.Get(id));
					}
					break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public View.Enumerator GetEnumerator()
		{
			return new View.Enumerator(Group);
		}
	}
}

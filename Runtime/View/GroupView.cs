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

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			Group.EnsureSynced();

			var groupSet = Group.MainSet;
			for (var i = Group.Count - 1; i >= 0; i--)
			{
				if (i > Group.Count)
				{
					i = Group.Count;
					continue;
				}

				if (!action.Apply(groupSet.Ids[i]))
				{
					break;
				}
			}
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			Group.EnsureSynced();

			var dataSet = Registry.DataSet<T>();

			var data = dataSet.Data;
			var groupSet = Group.MainSet;

			if (Group.IsOwning(dataSet))
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, Group.Count))
				{
					var page = data.Pages[pageIndex];
					for (int index = pageLength - 1; index >= 0; index--)
					{
						if (indexOffset + index > Group.Count)
						{
							index = Group.Count - indexOffset;
							continue;
						}

						int id = groupSet.Ids[indexOffset + index];
						if (!action.Apply(id, ref page[index]))
						{
							break;
						}
					}
				}
			}
			else
			{
				for (int index = Group.Count - 1; index >= 0; index--)
				{
					if (index > Group.Count)
					{
						index = Group.Count;
						continue;
					}

					int id = groupSet.Ids[index];
					if (!action.Apply(id, ref dataSet.Get(id)))
					{
						break;
					}
				}
			}
		}

		public void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			Group.EnsureSynced();

			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var groupSet = Group.MainSet;

			bool isOwning1 = Group.IsOwning(dataSet1);
			bool isOwning2 = Group.IsOwning(dataSet2);

			if (isOwning1 || isOwning2)
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
				{
					var page1 = isOwning1 ? data1.Pages[pageIndex] : null;
					var page2 = isOwning2 ? data2.Pages[pageIndex] : null;
					for (int index = pageLength - 1; index >= 0; index--)
					{
						if (indexOffset + index > Group.Count)
						{
							index = Group.Count - indexOffset;
							continue;
						}

						int id = groupSet.Ids[indexOffset + index];
						ref var val1 = ref isOwning1 ? ref page1[index] : ref dataSet1.Get(id);
						ref var val2 = ref isOwning2 ? ref page2[index] : ref dataSet2.Get(id);
						if (!action.Apply(id, ref val1, ref val2))
						{
							break;
						}
					}
				}
			}
			else
			{
				for (int index = Group.Count - 1; index >= 0; index--)
				{
					if (index > Group.Count)
					{
						index = Group.Count;
						continue;
					}

					int id = groupSet.Ids[index];
					if (!action.Apply(id, ref dataSet1.Get(id), ref dataSet2.Get(id)))
					{
						break;
					}
				}
			}
		}

		public void ForEach<TAction, T1, T2, T3>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			Group.EnsureSynced();

			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var groupSet = Group.MainSet;

			bool isOwning1 = Group.IsOwning(dataSet1);
			bool isOwning2 = Group.IsOwning(dataSet2);
			bool isOwning3 = Group.IsOwning(dataSet3);

			if (isOwning1 || isOwning2 || isOwning3)
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
				{
					var page1 = isOwning1 ? data1.Pages[pageIndex] : null;
					var page2 = isOwning2 ? data2.Pages[pageIndex] : null;
					var page3 = isOwning3 ? data3.Pages[pageIndex] : null;
					for (int index = pageLength - 1; index >= 0; index--)
					{
						if (indexOffset + index > Group.Count)
						{
							index = Group.Count - indexOffset;
							continue;
						}

						int id = groupSet.Ids[indexOffset + index];
						ref var val1 = ref isOwning1 ? ref page1[index] : ref dataSet1.Get(id);
						ref var val2 = ref isOwning2 ? ref page2[index] : ref dataSet2.Get(id);
						ref var val3 = ref isOwning3 ? ref page3[index] : ref dataSet3.Get(id);
						if (!action.Apply(id, ref val1, ref val2, ref val3))
						{
							break;
						}
					}
				}
			}
			else
			{
				for (int index = Group.Count - 1; index >= 0; index--)
				{
					if (index > Group.Count)
					{
						index = Group.Count;
						continue;
					}

					int id = groupSet.Ids[index];
					if (!action.Apply(id, ref dataSet1.Get(id), ref dataSet2.Get(id), ref dataSet3.Get(id)))
					{
						break;
					}
				}
			}
		}

		public void ForEach<TAction, T1, T2, T3, T4>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			Group.EnsureSynced();

			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();
			var dataSet4 = Registry.DataSet<T4>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var data4 = dataSet4.Data;
			var groupSet = Group.MainSet;

			bool isOwning1 = Group.IsOwning(dataSet1);
			bool isOwning2 = Group.IsOwning(dataSet2);
			bool isOwning3 = Group.IsOwning(dataSet3);
			bool isOwning4 = Group.IsOwning(dataSet4);

			if (isOwning1 || isOwning2 || isOwning3 || isOwning4)
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, Group.Count))
				{
					var page1 = isOwning1 ? data1.Pages[pageIndex] : null;
					var page2 = isOwning2 ? data2.Pages[pageIndex] : null;
					var page3 = isOwning3 ? data3.Pages[pageIndex] : null;
					var page4 = isOwning4 ? data4.Pages[pageIndex] : null;

					for (int index = pageLength - 1; index >= 0; index--)
					{
						if (indexOffset + index > Group.Count)
						{
							index = Group.Count - indexOffset;
							continue;
						}

						int id = groupSet.Ids[indexOffset + index];
						ref var val1 = ref isOwning1 ? ref page1[index] : ref dataSet1.Get(id);
						ref var val2 = ref isOwning2 ? ref page2[index] : ref dataSet2.Get(id);
						ref var val3 = ref isOwning3 ? ref page3[index] : ref dataSet3.Get(id);
						ref var val4 = ref isOwning4 ? ref page4[index] : ref dataSet4.Get(id);
						if (!action.Apply(id, ref val1, ref val2, ref val3, ref val4))
						{
							break;
						}
					}
				}
			}
			else
			{
				for (int index = Group.Count - 1; index >= 0; index--)
				{
					if (index > Group.Count)
					{
						index = Group.Count;
						continue;
					}

					int id = groupSet.Ids[index];
					if (!action.Apply(id, ref dataSet1.Get(id), ref dataSet2.Get(id), ref dataSet3.Get(id), ref dataSet4.Get(id)))
					{
						break;
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(Group);
		}

		public ref struct Enumerator
		{
			private readonly SparseSet _mainSet;
			private readonly IGroup _group;
			private int _index;

			public Enumerator(IGroup group)
			{
				_group = group;
				_mainSet = group.MainSet;
				_index = group.Count;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _mainSet.Ids[_index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_index > _group.Count)
				{
					_index = _group.Count - 1;
				}

				return _index >= 0;
			}
		}
	}
}

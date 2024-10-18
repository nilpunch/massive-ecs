using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public readonly struct View : IView
	{
		public Registry Registry { get; }

		public View(Registry registry)
		{
			Registry = registry;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			var entities = Registry.Entities;
			for (var i = entities.Count - 1; i >= 0; i--)
			{
				if (i > entities.Count)
				{
					i = entities.Count;
					continue;
				}

				if (!action.Apply(entities.Ids[i]))
				{
					break;
				}
			}
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			var dataSet = Registry.DataSet<T>();

			var data = dataSet.Data;

			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, dataSet.Count))
			{
				if (!data.HasPage(pageIndex))
				{
					continue;
				}

				var page = data.Pages[pageIndex];
				for (int packed = pageLength - 1; packed >= 0; packed--)
				{
					if (indexOffset + packed > dataSet.Count)
					{
						packed = dataSet.Count - indexOffset;
						continue;
					}

					var id = dataSet.Ids[indexOffset + packed];
					if (id >= 0)
					{
						if (!action.Apply(id, ref page[packed]))
						{
							break;
						}
					}
				}
			}
		}

		public void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;

			// Iterate over smallest data set
			if (dataSet1.Count <= dataSet2.Count)
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, dataSet1.Count))
				{
					if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int packed1 = pageLength - 1; packed1 >= 0; packed1--)
					{
						if (indexOffset + packed1 > dataSet1.Count)
						{
							packed1 = dataSet1.Count - indexOffset;
							continue;
						}

						int id = dataSet1.Ids[indexOffset + packed1];
						int packed2 = dataSet2.GetIndexOrInvalid(id);
						if (packed2 >= 0)
						{
							if (!action.Apply(id, ref page1[packed1], ref data2[packed2]))
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data2.PageSize, dataSet2.Count))
				{
					if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int packed2 = pageLength - 1; packed2 >= 0; packed2--)
					{
						if (indexOffset + packed2 > dataSet2.Count)
						{
							packed2 = dataSet2.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + packed2];
						int packed1 = dataSet1.GetIndexOrInvalid(id);
						if (packed1 >= 0)
						{
							if (!action.Apply(id, ref data1[packed1], ref page2[packed2]))
							{
								break;
							}
						}
					}
				}
			}
		}

		public void ForEach<TAction, T1, T2, T3>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;

			// Iterate over smallest data set
			if (dataSet1.Count <= dataSet2.Count && dataSet1.Count <= dataSet3.Count)
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, dataSet1.Count))
				{
					if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int packed1 = pageLength - 1; packed1 >= 0; packed1--)
					{
						if (indexOffset + packed1 > dataSet1.Count)
						{
							packed1 = dataSet1.Count - indexOffset;
							continue;
						}

						int id = dataSet1.Ids[indexOffset + packed1];
						int packed2 = dataSet2.GetIndexOrInvalid(id);
						int packed3 = dataSet3.GetIndexOrInvalid(id);
						if (packed2 >= 0 && packed3 >= 0)
						{
							if (!action.Apply(id, ref page1[packed1], ref data2[packed2], ref data3[packed3]))
							{
								break;
							}
						}
					}
				}
			}
			else if (dataSet2.Count <= dataSet1.Count && dataSet2.Count <= dataSet3.Count)
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data2.PageSize, dataSet2.Count))
				{
					if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int packed2 = pageLength - 1; packed2 >= 0; packed2--)
					{
						if (indexOffset + packed2 > dataSet2.Count)
						{
							packed2 = dataSet2.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + packed2];
						int packed1 = dataSet1.GetIndexOrInvalid(id);
						int packed3 = dataSet3.GetIndexOrInvalid(id);
						if (packed1 >= 0 && packed3 >= 0)
						{
							if (!action.Apply(id, ref data1[packed1], ref page2[packed2], ref data3[packed3]))
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data3.PageSize, dataSet3.Count))
				{
					if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page3 = data3.Pages[pageIndex];
					for (int packed3 = pageLength - 1; packed3 >= 0; packed3--)
					{
						if (indexOffset + packed3 > dataSet3.Count)
						{
							packed3 = dataSet3.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + packed3];
						int packed1 = dataSet1.GetIndexOrInvalid(id);
						int packed2 = dataSet2.GetIndexOrInvalid(id);
						if (packed1 >= 0 && packed2 >= 0)
						{
							if (!action.Apply(id, ref data1[packed1], ref data2[packed2], ref page3[packed3]))
							{
								break;
							}
						}
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Enumerator GetEnumerator()
		{
			return new Enumerator(Registry.Entities);
		}

		public ref struct Enumerator
		{
			private readonly IIdsSource _idsSource;
			private int _index;

			public Enumerator(IIdsSource idsSource)
			{
				_idsSource = idsSource;
				_index = idsSource.Count;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _idsSource.Ids[_index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_index > _idsSource.Count)
				{
					_index = _idsSource.Count - 1;
				}

				return _index >= 0;
			}
		}
	}
}

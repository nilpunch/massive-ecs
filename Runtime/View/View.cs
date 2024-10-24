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
				var page = data.Pages[pageIndex];
				for (int index = pageLength - 1; index >= 0; index--)
				{
					if (indexOffset + index > dataSet.Count)
					{
						index = dataSet.Count - indexOffset;
						continue;
					}

					var id = dataSet.Ids[indexOffset + index];
					if (id >= 0)
					{
						if (!action.Apply(id, ref page[index]))
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
					if (!data2.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int index1 = pageLength - 1; index1 >= 0; index1--)
					{
						if (indexOffset + index1 > dataSet1.Count)
						{
							index1 = dataSet1.Count - indexOffset;
							continue;
						}

						int id = dataSet1.Ids[indexOffset + index1];
						int index2 = dataSet2.GetIndexOrInvalid(id);
						if (index2 >= 0)
						{
							if (!action.Apply(id, ref page1[index1], ref data2[index2]))
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
					if (!data1.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int index2 = pageLength - 1; index2 >= 0; index2--)
					{
						if (indexOffset + index2 > dataSet2.Count)
						{
							index2 = dataSet2.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + index2];
						int index1 = dataSet1.GetIndexOrInvalid(id);
						if (index1 >= 0)
						{
							if (!action.Apply(id, ref data1[index1], ref page2[index2]))
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
					if (!data2.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int index1 = pageLength - 1; index1 >= 0; index1--)
					{
						if (indexOffset + index1 > dataSet1.Count)
						{
							index1 = dataSet1.Count - indexOffset;
							continue;
						}

						int id = dataSet1.Ids[indexOffset + index1];
						int index2 = dataSet2.GetIndexOrInvalid(id);
						int index3 = dataSet3.GetIndexOrInvalid(id);
						if (index2 >= 0 && index3 >= 0)
						{
							if (!action.Apply(id, ref page1[index1], ref data2[index2], ref data3[index3]))
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
					if (!data1.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int index2 = pageLength - 1; index2 >= 0; index2--)
					{
						if (indexOffset + index2 > dataSet2.Count)
						{
							index2 = dataSet2.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + index2];
						int index1 = dataSet1.GetIndexOrInvalid(id);
						int index3 = dataSet3.GetIndexOrInvalid(id);
						if (index1 >= 0 && index3 >= 0)
						{
							if (!action.Apply(id, ref data1[index1], ref page2[index2], ref data3[index3]))
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
					if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex))
					{
						continue;
					}

					var page3 = data3.Pages[pageIndex];
					for (int index3 = pageLength - 1; index3 >= 0; index3--)
					{
						if (indexOffset + index3 > dataSet3.Count)
						{
							index3 = dataSet3.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + index3];
						int index1 = dataSet1.GetIndexOrInvalid(id);
						int index2 = dataSet2.GetIndexOrInvalid(id);
						if (index1 >= 0 && index2 >= 0)
						{
							if (!action.Apply(id, ref data1[index1], ref data2[index2], ref page3[index3]))
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

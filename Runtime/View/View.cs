using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
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

		public void ForEach<TAction>(TAction action)
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

				action.Apply(entities.Ids[i]);
			}
		}

		public void ForEach<TAction, T>(TAction action)
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
				for (int dense = pageLength - 1; dense >= 0; dense--)
				{
					if (indexOffset + dense > dataSet.Count)
					{
						dense = dataSet.Count - indexOffset;
						continue;
					}

					var id = dataSet.Ids[indexOffset + dense];
					if (id != Constants.InvalidId)
					{
						action.Apply(id, ref page[dense]);
					}
				}
			}
		}

		public void ForEach<TAction, T1, T2>(TAction action)
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
					for (int dense1 = pageLength - 1; dense1 >= 0; dense1--)
					{
						if (indexOffset + dense1 > dataSet1.Count)
						{
							dense1 = dataSet1.Count - indexOffset;
							continue;
						}

						int id = dataSet1.Ids[indexOffset + dense1];
						int dense2 = dataSet2.GetDenseOrInvalid(id);
						if (dense2 != Constants.InvalidId)
						{
							action.Apply(id, ref page1[dense1], ref data2[dense2]);
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
					for (int dense2 = pageLength - 1; dense2 >= 0; dense2--)
					{
						if (indexOffset + dense2 > dataSet2.Count)
						{
							dense2 = dataSet2.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + dense2];
						int dense1 = dataSet1.GetDenseOrInvalid(id);
						if (dense1 != Constants.InvalidId)
						{
							action.Apply(id, ref data1[dense1], ref page2[dense2]);
						}
					}
				}
			}
		}

		public void ForEach<TAction, T1, T2, T3>(TAction action)
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
					for (int dense1 = pageLength - 1; dense1 >= 0; dense1--)
					{
						if (indexOffset + dense1 > dataSet1.Count)
						{
							dense1 = dataSet1.Count - indexOffset;
							continue;
						}

						int id = dataSet1.Ids[indexOffset + dense1];
						int dense2 = dataSet2.GetDenseOrInvalid(id);
						int dense3 = dataSet3.GetDenseOrInvalid(id);
						if (dense2 != Constants.InvalidId && dense3 != Constants.InvalidId)
						{
							action.Apply(id, ref page1[dense1], ref data2[dense2], ref data3[dense3]);
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
					for (int dense2 = pageLength - 1; dense2 >= 0; dense2--)
					{
						if (indexOffset + dense2 > dataSet2.Count)
						{
							dense2 = dataSet2.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + dense2];
						int dense1 = dataSet1.GetDenseOrInvalid(id);
						int dense3 = dataSet3.GetDenseOrInvalid(id);
						if (dense1 != Constants.InvalidId && dense3 != Constants.InvalidId)
						{
							action.Apply(id, ref data1[dense1], ref page2[dense2], ref data3[dense3]);
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
					for (int dense3 = pageLength - 1; dense3 >= 0; dense3--)
					{
						if (indexOffset + dense3 > dataSet3.Count)
						{
							dense3 = dataSet3.Count - indexOffset;
							continue;
						}

						int id = dataSet2.Ids[indexOffset + dense3];
						int dense1 = dataSet1.GetDenseOrInvalid(id);
						int dense2 = dataSet2.GetDenseOrInvalid(id);
						if (dense1 != Constants.InvalidId && dense2 != Constants.InvalidId)
						{
							action.Apply(id, ref data1[dense1], ref data2[dense2], ref page3[dense3]);
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
			private readonly Entities _entities;
			private int _index;

			public Enumerator(Entities entities)
			{
				_entities = entities;
				_index = entities.Count;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _entities.Ids[_index];
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				if (--_index > _entities.Count)
				{
					_index = _entities.Count - 1;
				}

				return _index >= 0;
			}
		}
	}
}

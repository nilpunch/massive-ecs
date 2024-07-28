using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public readonly struct View : IView
	{
		public IRegistry Registry { get; }

		public View(IRegistry registry)
		{
			Registry = registry;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction>(TAction action)
			where TAction : IEntityAction
		{
			var entities = Registry.Entities.Alive;
			for (var i = entities.Length - 1; i >= 0; i--)
			{
				action.Apply(entities[i].Id);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ForEach<TAction, T>(TAction action)
			where TAction : IEntityAction<T>
		{
			var dataSet = Registry.DataSet<T>();

			var data = dataSet.Data;
			var ids = dataSet.Ids;
			foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data.PageSize, dataSet.Count))
			{
				var page = data.Pages[pageIndex];
				for (int dense = pageLength - 1; dense >= 0; dense--)
				{
					var id = ids[indexOffset + dense];
					action.Apply(id, ref page[dense]);
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
				var ids1 = dataSet1.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, dataSet1.Count))
				{
					if (!data2.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int dense1 = pageLength - 1; dense1 >= 0; dense1--)
					{
						int id = ids1[indexOffset + dense1];
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
				var ids2 = dataSet2.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data2.PageSize, dataSet2.Count))
				{
					if (!data1.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int dense2 = pageLength - 1; dense2 >= 0; dense2--)
					{
						int id = ids2[indexOffset + dense2];
						int dense1 = dataSet1.GetDenseOrInvalid(id);
						if (dense1 != Constants.InvalidId)
						{
							action.Apply(id, ref data1[dense1], ref page2[dense2]);
						}
					}
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
				var ids1 = dataSet1.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data1.PageSize, dataSet1.Count))
				{
					if (!data2.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page1 = data1.Pages[pageIndex];
					for (int dense1 = pageLength - 1; dense1 >= 0; dense1--)
					{
						int id = ids1[indexOffset + dense1];
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
				var ids2 = dataSet2.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data2.PageSize, dataSet2.Count))
				{
					if (!data1.HasPage(pageIndex) || !data3.HasPage(pageIndex))
					{
						continue;
					}

					var page2 = data2.Pages[pageIndex];
					for (int dense2 = pageLength - 1; dense2 >= 0; dense2--)
					{
						int id = ids2[indexOffset + dense2];
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
				var ids3 = dataSet2.Ids;
				foreach (var (pageIndex, pageLength, indexOffset) in new PageSequence(data3.PageSize, dataSet3.Count))
				{
					if (!data1.HasPage(pageIndex) || !data2.HasPage(pageIndex))
					{
						continue;
					}

					var page3 = data3.Pages[pageIndex];
					for (int dense3 = pageLength - 1; dense3 >= 0; dense3--)
					{
						int id = ids3[indexOffset + dense3];
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
			return new Enumerator(Registry.Entities.Alive);
		}

		public ref struct Enumerator
		{
			private readonly ReadOnlySpan<int> _ids;
			private int _index;

			public Enumerator(ReadOnlySpan<Entity> entities)
			{
				_ids = MemoryMarshal.Cast<Entity, int>(entities);
				_index = _ids.Length / 2;
			}

			public int Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get => _ids[_index * 2] - Entity.IdOffset;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext()
			{
				return --_index >= 0;
			}
		}
	}
}

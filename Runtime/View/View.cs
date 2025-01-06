using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct View : IView
	{
		public Registry Registry { get; }
		public Packing PackingWhenIterating { get; }

		public View(Registry registry, Packing packingWhenIterating = Packing.WithHoles)
		{
			Registry = registry;
			PackingWhenIterating = packingWhenIterating;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			var entities = Registry.Entities;

			var originalPacking = entities.ExchangeToStricterPacking(PackingWhenIterating);

			for (var i = entities.Count - 1; i >= 0; i--)
			{
				if (i > entities.Count)
				{
					i = entities.Count;
					continue;
				}

				var id = entities.Packed[i];
				if (id >= 0)
				{
					if (!action.Apply(id))
					{
						break;
					}
				}
			}

			entities.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			var dataSet = Registry.DataSet<T>();

			var data = dataSet.Data;

			var originalPacking = dataSet.Packing;
			dataSet.ExchangeToStricterPacking(PackingWhenIterating);

			foreach (var page in new PageSequence(data.PageSize, dataSet.Count))
			{
				var dataPage = data.Pages[page.Index];
				for (var index = page.Length - 1; index >= 0; index--)
				{
					if (page.Offset + index > dataSet.Count)
					{
						index = dataSet.Count - page.Offset;
						continue;
					}

					var id = dataSet.Packed[page.Offset + index];
					if (id >= 0)
					{
						if (!action.Apply(id, ref dataPage[index]))
						{
							break;
						}
					}
				}
			}

			dataSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;

			var minSet = SetUtils.GetMinimalSet(dataSet1, dataSet2);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);

			// Iterate over the smallest data set.
			if (minSet == dataSet1)
			{
				foreach (var page in new PageSequence(data1.PageSize, dataSet1.Count))
				{
					if (!data2.HasPage(page.Index))
					{
						continue;
					}

					var page1 = data1.Pages[page.Index];
					for (var index1 = page.Length - 1; index1 >= 0; index1--)
					{
						if (page.Offset + index1 > dataSet1.Count)
						{
							index1 = dataSet1.Count - page.Offset;
							continue;
						}

						var id = dataSet1.Packed[page.Offset + index1];
						var index2 = dataSet2.GetIndexOrNegative(id);
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
				foreach (var page in new PageSequence(data2.PageSize, dataSet2.Count))
				{
					if (!data1.HasPage(page.Index))
					{
						continue;
					}

					var page2 = data2.Pages[page.Index];
					for (var index2 = page.Length - 1; index2 >= 0; index2--)
					{
						if (page.Offset + index2 > dataSet2.Count)
						{
							index2 = dataSet2.Count - page.Offset;
							continue;
						}

						var id = dataSet2.Packed[page.Offset + index2];
						var index1 = dataSet1.GetIndexOrNegative(id);
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

			minSet.ExchangePacking(originalPacking);
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

			var minSet = SetUtils.GetMinimalSet(dataSet1, dataSet2);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);

			// Iterate over the smallest data set.
			if (minSet == dataSet1)
			{
				foreach (var page in new PageSequence(data1.PageSize, dataSet1.Count))
				{
					if (!data2.HasPage(page.Index) || !data3.HasPage(page.Index))
					{
						continue;
					}

					var page1 = data1.Pages[page.Index];
					for (var index1 = page.Length - 1; index1 >= 0; index1--)
					{
						if (page.Offset + index1 > dataSet1.Count)
						{
							index1 = dataSet1.Count - page.Offset;
							continue;
						}

						var id = dataSet1.Packed[page.Offset + index1];
						var index2 = dataSet2.GetIndexOrNegative(id);
						var index3 = dataSet3.GetIndexOrNegative(id);
						if ((index2 | index3) >= 0)
						{
							if (!action.Apply(id, ref page1[index1], ref data2[index2], ref data3[index3]))
							{
								break;
							}
						}
					}
				}
			}
			else if (minSet == dataSet2)
			{
				foreach (var page in new PageSequence(data2.PageSize, dataSet2.Count))
				{
					if (!data1.HasPage(page.Index) || !data3.HasPage(page.Index))
					{
						continue;
					}

					var page2 = data2.Pages[page.Index];
					for (var index2 = page.Length - 1; index2 >= 0; index2--)
					{
						if (page.Offset + index2 > dataSet2.Count)
						{
							index2 = dataSet2.Count - page.Offset;
							continue;
						}

						var id = dataSet2.Packed[page.Offset + index2];
						var index1 = dataSet1.GetIndexOrNegative(id);
						var index3 = dataSet3.GetIndexOrNegative(id);
						if ((index1 | index3) >= 0)
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
				foreach (var page in new PageSequence(data3.PageSize, dataSet3.Count))
				{
					if (!data1.HasPage(page.Index) || !data2.HasPage(page.Index))
					{
						continue;
					}

					var page3 = data3.Pages[page.Index];
					for (var index3 = page.Length - 1; index3 >= 0; index3--)
					{
						if (page.Offset + index3 > dataSet3.Count)
						{
							index3 = dataSet3.Count - page.Offset;
							continue;
						}

						var id = dataSet2.Packed[page.Offset + index3];
						var index1 = dataSet1.GetIndexOrNegative(id);
						var index2 = dataSet2.GetIndexOrNegative(id);
						if ((index1 | index2) >= 0)
						{
							if (!action.Apply(id, ref data1[index1], ref data2[index2], ref page3[index3]))
							{
								break;
							}
						}
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T1, T2, T3, T4>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();
			var dataSet4 = Registry.DataSet<T4>();

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var data4 = dataSet4.Data;

			var minSet = SetUtils.GetMinimalSet(dataSet1, dataSet2);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);

			// Iterate over the smallest data set.
			if (minSet == dataSet1)
			{
				foreach (var page in new PageSequence(data1.PageSize, dataSet1.Count))
				{
					if (!data2.HasPage(page.Index) || !data3.HasPage(page.Index) || !data4.HasPage(page.Index))
					{
						continue;
					}

					var page1 = data1.Pages[page.Index];
					for (var index1 = page.Length - 1; index1 >= 0; index1--)
					{
						if (page.Offset + index1 > dataSet1.Count)
						{
							index1 = dataSet1.Count - page.Offset;
							continue;
						}

						var id = dataSet1.Packed[page.Offset + index1];
						var index2 = dataSet2.GetIndexOrNegative(id);
						var index3 = dataSet3.GetIndexOrNegative(id);
						var index4 = dataSet4.GetIndexOrNegative(id);
						if ((index2 | index3 | index4) >= 0)
						{
							if (!action.Apply(id, ref page1[index1], ref data2[index2], ref data3[index3], ref data4[index4]))
							{
								break;
							}
						}
					}
				}
			}
			else if (minSet == dataSet2)
			{
				foreach (var page in new PageSequence(data2.PageSize, dataSet2.Count))
				{
					if (!data1.HasPage(page.Index) || !data3.HasPage(page.Index) || !data4.HasPage(page.Index))
					{
						continue;
					}

					var page2 = data2.Pages[page.Index];
					for (var index2 = page.Length - 1; index2 >= 0; index2--)
					{
						if (page.Offset + index2 > dataSet2.Count)
						{
							index2 = dataSet2.Count - page.Offset;
							continue;
						}

						var id = dataSet2.Packed[page.Offset + index2];
						var index1 = dataSet1.GetIndexOrNegative(id);
						var index3 = dataSet3.GetIndexOrNegative(id);
						var index4 = dataSet4.GetIndexOrNegative(id);
						if ((index1 | index3 | index4) >= 0)
						{
							if (!action.Apply(id, ref data1[index1], ref page2[index2], ref data3[index3], ref data4[index4]))
							{
								break;
							}
						}
					}
				}
			}
			else if (minSet == dataSet3)
			{
				foreach (var page in new PageSequence(data3.PageSize, dataSet3.Count))
				{
					if (!data1.HasPage(page.Index) || !data2.HasPage(page.Index) || !data4.HasPage(page.Index))
					{
						continue;
					}

					var page3 = data3.Pages[page.Index];
					for (var index3 = page.Length - 1; index3 >= 0; index3--)
					{
						if (page.Offset + index3 > dataSet3.Count)
						{
							index3 = dataSet3.Count - page.Offset;
							continue;
						}

						var id = dataSet3.Packed[page.Offset + index3];
						var index1 = dataSet1.GetIndexOrNegative(id);
						var index2 = dataSet2.GetIndexOrNegative(id);
						var index4 = dataSet4.GetIndexOrNegative(id);
						if ((index1 | index2 | index4) >= 0)
						{
							if (!action.Apply(id, ref data1[index1], ref data2[index2], ref page3[index3], ref data4[index4]))
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				foreach (var page in new PageSequence(data4.PageSize, dataSet4.Count))
				{
					if (!data1.HasPage(page.Index) || !data2.HasPage(page.Index) || !data3.HasPage(page.Index))
					{
						continue;
					}

					var page4 = data4.Pages[page.Index];
					for (var index4 = page.Length - 1; index4 >= 0; index4--)
					{
						if (page.Offset + index4 > dataSet4.Count)
						{
							index4 = dataSet4.Count - page.Offset;
							continue;
						}

						var id = dataSet4.Packed[page.Offset + index4];
						var index1 = dataSet1.GetIndexOrNegative(id);
						var index2 = dataSet2.GetIndexOrNegative(id);
						var index3 = dataSet3.GetIndexOrNegative(id);
						if ((index1 | index2 | index3) >= 0)
						{
							if (!action.Apply(id, ref data1[index1], ref data2[index2], ref data3[index3], ref page4[index4]))
							{
								break;
							}
						}
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PackedEnumerator GetEnumerator()
		{
			return new PackedEnumerator(Registry.Entities, PackingWhenIterating);
		}
	}
}

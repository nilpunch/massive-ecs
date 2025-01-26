﻿#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct FilterView : IView
	{
		public Registry Registry { get; }
		public Filter Filter { get; }
		public Packing PackingWhenIterating { get; }

		public FilterView(Registry registry, Filter filter = null,
			Packing packingWhenIterating = Packing.WithHoles)
		{
			Registry = registry;
			PackingWhenIterating = packingWhenIterating;
			Filter = filter ?? registry.FilterRegistry.Empty;
		}

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			PackedSet packedSet;
			ReducedFilter reducedFilter;
			if (Filter.Included.Length == 0)
			{
				packedSet = Registry.Entities;
				reducedFilter = Filter.NotReduced;
			}
			else
			{
				var minimalSet = SetUtils.GetMinimalSet(Filter.Included);
				packedSet = minimalSet;
				reducedFilter = Filter.ReduceIncluded(minimalSet);
			}

			var originalPacking = packedSet.ExchangeToStricterPacking(PackingWhenIterating);

			for (var i = packedSet.Count - 1; i >= 0; i--)
			{
				if (i > packedSet.Count)
				{
					i = packedSet.Count;
					continue;
				}

				var id = packedSet.Packed[i];
				if (reducedFilter.ContainsId(id))
				{
					if (!action.Apply(id))
					{
						break;
					}
				}
			}

			packedSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			Assert.TypeHasData<T>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);

			var dataSet = Registry.DataSet<T>();

			ThrowIfCantInclude(dataSet);

			var data = dataSet.Data;

			var minSet = SetUtils.GetMinimalSet(dataSet, Filter.Included);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);
			var reducedFilter = Filter.ReduceIncluded(minSet);

			if (minSet == dataSet)
			{
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
						if (reducedFilter.ContainsId(id))
						{
							if (!action.Apply(id, ref dataPage[index]))
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				for (var i = minSet.Count - 1; i >= 0; i--)
				{
					if (i > minSet.Count)
					{
						i = minSet.Count;
						continue;
					}

					var id = minSet.Packed[i];
					var index = dataSet.GetIndexOrNegative(id);
					if (index >= 0 && reducedFilter.ContainsId(id))
					{
						if (!action.Apply(id, ref data[index]))
						{
							break;
						}
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T1, T2>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			Assert.TypeHasData<T1>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);
			Assert.TypeHasData<T2>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);

			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();

			ThrowIfCantInclude(dataSet1);
			ThrowIfCantInclude(dataSet2);

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var minDataSet = SetUtils.GetMinimalSet(dataSet1, dataSet2);

			var minSet = SetUtils.GetMinimalSet(minDataSet, Filter.Included);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);
			var reducedFilter = Filter.ReduceIncluded(minSet);

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
						if (index2 >= 0 && reducedFilter.ContainsId(id))
						{
							if (!action.Apply(id, ref page1[index1], ref data2[index2]))
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
						if (index1 >= 0 && reducedFilter.ContainsId(id))
						{
							if (!action.Apply(id, ref data1[index1], ref page2[index2]))
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				for (var i = minSet.Count - 1; i >= 0; i--)
				{
					if (i > minSet.Count)
					{
						i = minSet.Count;
						continue;
					}

					var id = minSet.Packed[i];
					var index1 = dataSet1.GetIndexOrNegative(id);
					var index2 = dataSet2.GetIndexOrNegative(id);
					if ((index1 | index2) >= 0
						&& reducedFilter.ContainsId(id))
					{
						if (!action.Apply(id, ref data1[index1], ref data2[index2]))
						{
							break;
						}
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T1, T2, T3>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			Assert.TypeHasData<T1>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);
			Assert.TypeHasData<T2>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);
			Assert.TypeHasData<T3>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);

			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();

			ThrowIfCantInclude(dataSet1);
			ThrowIfCantInclude(dataSet2);
			ThrowIfCantInclude(dataSet3);

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var minDataSet = SetUtils.GetMinimalSet(dataSet1, dataSet2, dataSet3);

			var minSet = SetUtils.GetMinimalSet(minDataSet, Filter.Included);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);
			var reducedFilter = Filter.ReduceIncluded(minSet);

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
						if ((index2 | index3) >= 0
							&& reducedFilter.ContainsId(id))
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
						if ((index1 | index3) >= 0
							&& reducedFilter.ContainsId(id))
						{
							if (!action.Apply(id, ref data1[index1], ref page2[index2], ref data3[index3]))
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
						if ((index1 | index2) >= 0
							&& reducedFilter.ContainsId(id))
						{
							if (!action.Apply(id, ref data1[index1], ref data2[index2], ref page3[index3]))
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				for (var i = minSet.Count - 1; i >= 0; i--)
				{
					if (i > minSet.Count)
					{
						i = minSet.Count;
						continue;
					}

					var id = minSet.Packed[i];
					var index1 = dataSet1.GetIndexOrNegative(id);
					var index2 = dataSet2.GetIndexOrNegative(id);
					var index3 = dataSet3.GetIndexOrNegative(id);
					if ((index1 | index2 | index3) >= 0
						&& reducedFilter.ContainsId(id))
					{
						if (!action.Apply(id, ref data1[index1], ref data2[index2], ref data3[index3]))
						{
							break;
						}
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		public void ForEach<TAction, T1, T2, T3, T4>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			Assert.TypeHasData<T1>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);
			Assert.TypeHasData<T2>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);
			Assert.TypeHasData<T3>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);
			Assert.TypeHasData<T4>(Registry, SuggestionMessage.DontUseViewsWithEmptyTypes);

			var dataSet1 = Registry.DataSet<T1>();
			var dataSet2 = Registry.DataSet<T2>();
			var dataSet3 = Registry.DataSet<T3>();
			var dataSet4 = Registry.DataSet<T4>();

			ThrowIfCantInclude(dataSet1);
			ThrowIfCantInclude(dataSet2);
			ThrowIfCantInclude(dataSet3);
			ThrowIfCantInclude(dataSet4);

			var data1 = dataSet1.Data;
			var data2 = dataSet2.Data;
			var data3 = dataSet3.Data;
			var data4 = dataSet4.Data;
			var minDataSet = SetUtils.GetMinimalSet(dataSet1, dataSet2, dataSet3, dataSet4);

			var minSet = SetUtils.GetMinimalSet(minDataSet, Filter.Included);
			var originalPacking = minSet.ExchangeToStricterPacking(PackingWhenIterating);
			var reducedFilter = Filter.ReduceIncluded(minSet);

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
						if ((index2 | index3 | index4) >= 0
							&& reducedFilter.ContainsId(id))
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
						if ((index1 | index3 | index4) >= 0
							&& reducedFilter.ContainsId(id))
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
						if ((index1 | index2 | index4) >= 0
							&& reducedFilter.ContainsId(id))
						{
							if (!action.Apply(id, ref data1[index1], ref data2[index2], ref page3[index3], ref data4[index4]))
							{
								break;
							}
						}
					}
				}
			}
			else if (minSet == dataSet4)
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
						if ((index1 | index2 | index3) >= 0
							&& reducedFilter.ContainsId(id))
						{
							if (!action.Apply(id, ref data1[index1], ref data2[index2], ref data3[index3], ref page4[index4]))
							{
								break;
							}
						}
					}
				}
			}
			else
			{
				for (var i = minSet.Count - 1; i >= 0; i--)
				{
					if (i > minSet.Count)
					{
						i = minSet.Count;
						continue;
					}

					var id = minSet.Packed[i];
					var index1 = dataSet1.GetIndexOrNegative(id);
					var index2 = dataSet2.GetIndexOrNegative(id);
					var index3 = dataSet3.GetIndexOrNegative(id);
					var index4 = dataSet4.GetIndexOrNegative(id);
					if ((index1 | index2 | index3 | index4) >= 0
						&& reducedFilter.ContainsId(id))
					{
						if (!action.Apply(id, ref data1[index1], ref data2[index2], ref data3[index3], ref data4[index4]))
						{
							break;
						}
					}
				}
			}

			minSet.ExchangePacking(originalPacking);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PackedFilterEnumerator GetEnumerator()
		{
			if (Filter.Included.Length == 0)
			{
				return new PackedFilterEnumerator(Registry.Entities, Filter.NotReduced, PackingWhenIterating);
			}
			else
			{
				var minimalSet = SetUtils.GetMinimalSet(Filter.Included);
				return new PackedFilterEnumerator(minimalSet, Filter.ReduceIncluded(minimalSet), PackingWhenIterating);
			}
		}

		[Conditional(Assert.Symbol)]
		private void ThrowIfCantInclude(SparseSet sparseSet)
		{
			if (Filter.Excluded.Contains(sparseSet))
			{
				throw new Exception($"Conflicting exclude filter and {sparseSet.GetType().GetGenericName()}!");
			}
		}
	}
}

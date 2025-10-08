#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using Unity.IL2CPP.CompilerServices;

// ReSharper disable FieldHidesInterfacePropertyWithDefaultImplementation

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct Query : IQueryable
	{
		public World World;
		public Filter Filter;

		public Query(World world)
		{
			World = world;
			Filter = Filter.Empty;
		}

		public Query(World world, Filter filter)
		{
			World = world;
			Filter = filter;
		}

		Query IQueryable.Query => this;

		public EntityEnumerable Entities => new EntityEnumerable(RentCacheAndPrepare(), World);

		public void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction
		{
			var cache = RentCacheAndPrepare();

			var deBruijn = MathUtils.DeBruijn;
			var nonEmptyBitsCount = cache.NonEmptyBitsCount;
			var nonEmptyBitsIndices = cache.NonEmptyBitsIndices;
			var cachedBits = cache.Bits;
			for (var i = 0; i < nonEmptyBitsCount; i++)
			{
				var bitsIndex = nonEmptyBitsIndices[i];
				ref var bitsRef = ref cachedBits[bitsIndex];
				var bits = bitsRef;

				if (bits == 0UL)
				{
					continue;
				}

				var bitsOffset = bitsIndex << 6;
				var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];

				var runEnd = MathUtils.ApproximateMSB(bits);
				var setBits = MathUtils.PopCount(bits);
				if (setBits << 1 > runEnd - bit)
				{
					for (; bit < runEnd; bit++)
					{
						if ((bitsRef & (1UL << bit)) == 0UL)
						{
							continue;
						}

						action.Apply(bitsOffset + bit);
					}
				}
				else
				{
					do
					{
						action.Apply(bitsOffset + bit);
						bits &= (bits - 1UL) & bitsRef;
						bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					} while (bits != 0UL);
				}
			}

			QueryCache.ReturnAndPop(cache);
		}

		public void ForEach<T, TAction>(ref TAction action)
			where TAction : IEntityAction<T>
		{
			NoDataException.ThrowIfHasNoData<T>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T>();

			FilterException.ThrowIfCantQuery<T>(Filter, dataSet1);

			var cache = QueryCache.Rent()
				.AddInclude(dataSet1);

			ApplyFilter(Filter, cache);

			cache.Update();

			var deBruijn = MathUtils.DeBruijn;
			var nonEmptyBitsCount = cache.NonEmptyBitsCount;
			var nonEmptyBitsIndices = cache.NonEmptyBitsIndices;
			var cachedBits = cache.Bits;
			for (var i = 0; i < nonEmptyBitsCount; i++)
			{
				var bitsIndex = nonEmptyBitsIndices[i];
				ref var bitsRef = ref cachedBits[bitsIndex];
				var bits = bitsRef;

				if (bits == 0UL)
				{
					continue;
				}

				var bitsOffset = bitsIndex << 6;
				var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
				var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
				var pageIndex = bitsOffset >> Constants.PageSizePower;
				var dataPage1 = dataSet1.PagedData[pageIndex];

				var runEnd = MathUtils.ApproximateMSB(bits);
				var setBits = MathUtils.PopCount(bits);
				if (setBits << 1 > runEnd - bit)
				{
					for (; bit < runEnd; bit++)
					{
						if ((bitsRef & (1UL << bit)) == 0UL)
						{
							continue;
						}

						action.Apply(bitsOffset + bit,
							ref dataPage1[dataOffset + bit]);
					}
				}
				else
				{
					do
					{
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataOffset + bit]);
						bits &= (bits - 1UL) & bitsRef;
						bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					} while (bits != 0UL);
				}
			}

			QueryCache.ReturnAndPop(cache);
		}

		public void ForEach<T1, T2, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2>
		{
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();

			FilterException.ThrowIfCantQuery<T1>(Filter, dataSet1);
			FilterException.ThrowIfCantQuery<T2>(Filter, dataSet2);

			var cache = QueryCache.Rent()
				.AddInclude(dataSet1)
				.AddInclude(dataSet2);

			ApplyFilter(Filter, cache);

			cache.Update();

			var deBruijn = MathUtils.DeBruijn;
			var nonEmptyBitsCount = cache.NonEmptyBitsCount;
			var nonEmptyBitsIndices = cache.NonEmptyBitsIndices;
			var cachedBits = cache.Bits;
			for (var i = 0; i < nonEmptyBitsCount; i++)
			{
				var bitsIndex = nonEmptyBitsIndices[i];
				ref var bitsRef = ref cachedBits[bitsIndex];
				var bits = bitsRef;

				if (bits == 0UL)
				{
					continue;
				}

				var bitsOffset = bitsIndex << 6;
				var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
				var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
				var pageIndex = bitsOffset >> Constants.PageSizePower;
				var dataPage1 = dataSet1.PagedData[pageIndex];
				var dataPage2 = dataSet2.PagedData[pageIndex];

				var runEnd = MathUtils.ApproximateMSB(bits);
				var setBits = MathUtils.PopCount(bits);
				if (setBits << 1 > runEnd - bit)
				{
					for (; bit < runEnd; bit++)
					{
						if ((bitsRef & (1UL << bit)) == 0UL)
						{
							continue;
						}

						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex]);
					}
				}
				else
				{
					do
					{
						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex]);
						bits &= (bits - 1UL) & bitsRef;
						bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					} while (bits != 0UL);
				}
			}

			QueryCache.ReturnAndPop(cache);
		}

		public void ForEach<T1, T2, T3, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3>
		{
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();
			var dataSet3 = World.DataSet<T3>();

			FilterException.ThrowIfCantQuery<T1>(Filter, dataSet1);
			FilterException.ThrowIfCantQuery<T2>(Filter, dataSet2);
			FilterException.ThrowIfCantQuery<T3>(Filter, dataSet3);

			var cache = QueryCache.Rent()
				.AddInclude(dataSet1)
				.AddInclude(dataSet2)
				.AddInclude(dataSet3);

			ApplyFilter(Filter, cache);

			cache.Update();

			var deBruijn = MathUtils.DeBruijn;
			var nonEmptyBitsCount = cache.NonEmptyBitsCount;
			var nonEmptyBitsIndices = cache.NonEmptyBitsIndices;
			var cachedBits = cache.Bits;
			for (var i = 0; i < nonEmptyBitsCount; i++)
			{
				var bitsIndex = nonEmptyBitsIndices[i];
				ref var bitsRef = ref cachedBits[bitsIndex];
				var bits = bitsRef;

				if (bits == 0UL)
				{
					continue;
				}

				var bitsOffset = bitsIndex << 6;
				var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
				var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
				var pageIndex = bitsOffset >> Constants.PageSizePower;
				var dataPage1 = dataSet1.PagedData[pageIndex];
				var dataPage2 = dataSet2.PagedData[pageIndex];
				var dataPage3 = dataSet3.PagedData[pageIndex];

				var runEnd = MathUtils.ApproximateMSB(bits);
				var setBits = MathUtils.PopCount(bits);
				if (setBits << 1 > runEnd - bit)
				{
					for (; bit < runEnd; bit++)
					{
						if ((bitsRef & (1UL << bit)) == 0UL)
						{
							continue;
						}

						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex]);
					}
				}
				else
				{
					do
					{
						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex]);
						bits &= (bits - 1UL) & bitsRef;
						bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					} while (bits != 0UL);
				}
			}

			QueryCache.ReturnAndPop(cache);
		}

		public void ForEach<T1, T2, T3, T4, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4>
		{
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T4>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();
			var dataSet3 = World.DataSet<T3>();
			var dataSet4 = World.DataSet<T4>();

			FilterException.ThrowIfCantQuery<T1>(Filter, dataSet1);
			FilterException.ThrowIfCantQuery<T2>(Filter, dataSet2);
			FilterException.ThrowIfCantQuery<T3>(Filter, dataSet3);
			FilterException.ThrowIfCantQuery<T4>(Filter, dataSet4);

			var cache = QueryCache.Rent()
				.AddInclude(dataSet1)
				.AddInclude(dataSet2)
				.AddInclude(dataSet3)
				.AddInclude(dataSet4);

			ApplyFilter(Filter, cache);

			cache.Update();

			var deBruijn = MathUtils.DeBruijn;
			var nonEmptyBitsCount = cache.NonEmptyBitsCount;
			var nonEmptyBitsIndices = cache.NonEmptyBitsIndices;
			var cachedBits = cache.Bits;
			for (var i = 0; i < nonEmptyBitsCount; i++)
			{
				var bitsIndex = nonEmptyBitsIndices[i];
				ref var bitsRef = ref cachedBits[bitsIndex];
				var bits = bitsRef;

				if (bits == 0UL)
				{
					continue;
				}

				var bitsOffset = bitsIndex << 6;
				var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
				var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
				var pageIndex = bitsOffset >> Constants.PageSizePower;
				var dataPage1 = dataSet1.PagedData[pageIndex];
				var dataPage2 = dataSet2.PagedData[pageIndex];
				var dataPage3 = dataSet3.PagedData[pageIndex];
				var dataPage4 = dataSet4.PagedData[pageIndex];

				var runEnd = MathUtils.ApproximateMSB(bits);
				var setBits = MathUtils.PopCount(bits);
				if (setBits << 1 > runEnd - bit)
				{
					for (; bit < runEnd; bit++)
					{
						if ((bitsRef & (1UL << bit)) == 0UL)
						{
							continue;
						}

						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex],
							ref dataPage4[dataIndex]);
					}
				}
				else
				{
					do
					{
						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex],
							ref dataPage4[dataIndex]);
						bits &= (bits - 1UL) & bitsRef;
						bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					} while (bits != 0UL);
				}
			}

			QueryCache.ReturnAndPop(cache);
		}

		public void ForEach<T1, T2, T3, T4, T5, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4, T5>
		{
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T4>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T5>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();
			var dataSet3 = World.DataSet<T3>();
			var dataSet4 = World.DataSet<T4>();
			var dataSet5 = World.DataSet<T5>();

			FilterException.ThrowIfCantQuery<T1>(Filter, dataSet1);
			FilterException.ThrowIfCantQuery<T2>(Filter, dataSet2);
			FilterException.ThrowIfCantQuery<T3>(Filter, dataSet3);
			FilterException.ThrowIfCantQuery<T4>(Filter, dataSet4);
			FilterException.ThrowIfCantQuery<T5>(Filter, dataSet5);

			var cache = QueryCache.Rent()
				.AddInclude(dataSet1)
				.AddInclude(dataSet2)
				.AddInclude(dataSet3)
				.AddInclude(dataSet4)
				.AddInclude(dataSet5);

			ApplyFilter(Filter, cache);

			cache.Update();

			var deBruijn = MathUtils.DeBruijn;
			var nonEmptyBitsCount = cache.NonEmptyBitsCount;
			var nonEmptyBitsIndices = cache.NonEmptyBitsIndices;
			var cachedBits = cache.Bits;
			for (var i = 0; i < nonEmptyBitsCount; i++)
			{
				var bitsIndex = nonEmptyBitsIndices[i];
				ref var bitsRef = ref cachedBits[bitsIndex];
				var bits = bitsRef;

				if (bits == 0UL)
				{
					continue;
				}

				var bitsOffset = bitsIndex << 6;
				var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
				var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
				var pageIndex = bitsOffset >> Constants.PageSizePower;
				var dataPage1 = dataSet1.PagedData[pageIndex];
				var dataPage2 = dataSet2.PagedData[pageIndex];
				var dataPage3 = dataSet3.PagedData[pageIndex];
				var dataPage4 = dataSet4.PagedData[pageIndex];
				var dataPage5 = dataSet5.PagedData[pageIndex];

				var runEnd = MathUtils.ApproximateMSB(bits);
				var setBits = MathUtils.PopCount(bits);
				if (setBits << 1 > runEnd - bit)
				{
					for (; bit < runEnd; bit++)
					{
						if ((bitsRef & (1UL << bit)) == 0UL)
						{
							continue;
						}

						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex],
							ref dataPage4[dataIndex],
							ref dataPage5[dataIndex]);
					}
				}
				else
				{
					do
					{
						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex],
							ref dataPage4[dataIndex],
							ref dataPage5[dataIndex]);
						bits &= (bits - 1UL) & bitsRef;
						bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					} while (bits != 0UL);
				}
			}

			QueryCache.ReturnAndPop(cache);
		}

		public void ForEach<T1, T2, T3, T4, T5, T6, TAction>(ref TAction action)
			where TAction : IEntityAction<T1, T2, T3, T4, T5, T6>
		{
			NoDataException.ThrowIfHasNoData<T1>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T2>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T3>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T4>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T5>(World, DataAccessContext.View);
			NoDataException.ThrowIfHasNoData<T6>(World, DataAccessContext.View);

			var dataSet1 = World.DataSet<T1>();
			var dataSet2 = World.DataSet<T2>();
			var dataSet3 = World.DataSet<T3>();
			var dataSet4 = World.DataSet<T4>();
			var dataSet5 = World.DataSet<T5>();
			var dataSet6 = World.DataSet<T6>();

			FilterException.ThrowIfCantQuery<T1>(Filter, dataSet1);
			FilterException.ThrowIfCantQuery<T2>(Filter, dataSet2);
			FilterException.ThrowIfCantQuery<T3>(Filter, dataSet3);
			FilterException.ThrowIfCantQuery<T4>(Filter, dataSet4);
			FilterException.ThrowIfCantQuery<T5>(Filter, dataSet5);
			FilterException.ThrowIfCantQuery<T6>(Filter, dataSet6);

			var cache = QueryCache.Rent()
				.AddInclude(dataSet1)
				.AddInclude(dataSet2)
				.AddInclude(dataSet3)
				.AddInclude(dataSet4)
				.AddInclude(dataSet5)
				.AddInclude(dataSet6);

			ApplyFilter(Filter, cache);

			cache.Update();

			var deBruijn = MathUtils.DeBruijn;
			var nonEmptyBitsCount = cache.NonEmptyBitsCount;
			var nonEmptyBitsIndices = cache.NonEmptyBitsIndices;
			var cachedBits = cache.Bits;
			for (var i = 0; i < nonEmptyBitsCount; i++)
			{
				var bitsIndex = nonEmptyBitsIndices[i];
				ref var bitsRef = ref cachedBits[bitsIndex];
				var bits = bitsRef;

				if (bits == 0UL)
				{
					continue;
				}

				var bitsOffset = bitsIndex << 6;
				var bit = (int)deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
				var dataOffset = bitsOffset & Constants.PageSizeMinusOne;
				var pageIndex = bitsOffset >> Constants.PageSizePower;
				var dataPage1 = dataSet1.PagedData[pageIndex];
				var dataPage2 = dataSet2.PagedData[pageIndex];
				var dataPage3 = dataSet3.PagedData[pageIndex];
				var dataPage4 = dataSet4.PagedData[pageIndex];
				var dataPage5 = dataSet5.PagedData[pageIndex];
				var dataPage6 = dataSet6.PagedData[pageIndex];

				var runEnd = MathUtils.ApproximateMSB(bits);
				var setBits = MathUtils.PopCount(bits);
				if (setBits << 1 > runEnd - bit)
				{
					for (; bit < runEnd; bit++)
					{
						if ((bitsRef & (1UL << bit)) == 0UL)
						{
							continue;
						}

						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex],
							ref dataPage4[dataIndex],
							ref dataPage5[dataIndex],
							ref dataPage6[dataIndex]);
					}
				}
				else
				{
					do
					{
						var dataIndex = dataOffset + bit;
						action.Apply(bitsOffset + bit,
							ref dataPage1[dataIndex],
							ref dataPage2[dataIndex],
							ref dataPage3[dataIndex],
							ref dataPage4[dataIndex],
							ref dataPage5[dataIndex],
							ref dataPage6[dataIndex]);
						bits &= (bits - 1UL) & bitsRef;
						bit = deBruijn[(int)(((bits & (ulong)-(long)bits) * 0x37E84A99DAE458FUL) >> 58)];
					} while (bits != 0UL);
				}
			}

			QueryCache.ReturnAndPop(cache);
		}

		public IdsEnumerator GetEnumerator()
		{
			return new IdsEnumerator(RentCacheAndPrepare());
		}

		private QueryCache RentCacheAndPrepare()
		{
			var cache = QueryCache.Rent();

			if (Filter.IncludeCount == 0)
			{
				cache.AddInclude(World.Entities);
			}

			ApplyFilter(Filter, cache);

			cache.Update();

			return cache;
		}

		private void ApplyFilter(Filter filter, QueryCache resultQueryCache)
		{
			for (var i = 0; i < filter.IncludeCount; i++)
			{
				var included = filter.Included[i];
				resultQueryCache.AddInclude(included);
			}

			for (var i = 0; i < filter.ExcludeCount; i++)
			{
				var excluded = filter.Excluded[i];
				resultQueryCache.AddExclude(excluded);
			}
		}
	}
}

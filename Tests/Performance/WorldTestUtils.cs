namespace Massive.PerformanceTests
{
	public static class WorldTestUtils
	{
		public static World FillWorldWith50Components(this World world)
		{
			world.Sparse<TestState64>();
			world.Sparse<TestState64_2>();
			world.Sparse<TestState64_3>();
			world.Sparse<TestState64<double, int, long>>();
			world.Sparse<TestState64<long, short, byte>>();
			world.Sparse<TestState64<short, ushort, int>>();
			world.Sparse<TestState64<ushort, ulong, float>>();
			world.Sparse<TestState64<ulong, double, decimal>>();
			world.Sparse<TestState64<decimal, char, byte>>();
			world.Sparse<TestState64<char, bool, int>>();
			world.Sparse<TestState64<bool, byte, sbyte>>();
			world.Sparse<TestState64<sbyte, float, ushort>>();
			world.Sparse<TestState64<int, double, bool>>();
			world.Sparse<TestState64<double, decimal, char>>();
			world.Sparse<TestState64<long, bool, float>>();
			world.Sparse<TestState64<short, int, ulong>>();
			world.Sparse<TestState64<ushort, sbyte, decimal>>();
			world.Sparse<TestState64<ulong, char, double>>();
			world.Sparse<TestState64<decimal, bool, short>>();
			world.Sparse<TestState64<char, int, byte>>();
			world.Sparse<TestState64<bool, ulong, sbyte>>();
			world.Sparse<TestState64<sbyte, short, ushort>>();
			world.Sparse<TestState64<float, ulong, int>>();
			world.Sparse<TestState64<double, sbyte, long>>();
			world.Sparse<TestState64<int, char, decimal>>();
			world.Sparse<TestState64<long, decimal, bool>>();
			world.Sparse<TestState64<short, double, byte>>();
			world.Sparse<TestState64<ushort, float, char>>();
			world.Sparse<TestState64<ulong, int, bool>>();
			world.Sparse<TestState64<decimal, short, ulong>>();
			world.Sparse<TestState64<char, ushort, float>>();
			world.Sparse<TestState64<bool, double, sbyte>>();
			world.Sparse<TestState64<sbyte, long, ushort>>();
			world.Sparse<TestState64<float, decimal, char>>();
			world.Sparse<TestState64<double, byte, int>>();
			world.Sparse<TestState64<int, sbyte, ulong>>();
			world.Sparse<TestState64<long, ushort, decimal>>();
			world.Sparse<TestState64<short, char, double>>();
			world.Sparse<TestState64<ushort, bool, float>>();
			world.Sparse<TestState64<ulong, byte, short>>();
			world.Sparse<TestState64<decimal, int, char>>();
			world.Sparse<TestState64<char, ulong, bool>>();
			world.Sparse<TestState64<bool, short, double>>();
			world.Sparse<TestState64<sbyte, decimal, ushort>>();
			world.Sparse<TestState64<float, bool, byte>>();
			world.Sparse<TestState64<double, ushort, sbyte>>();
			world.Sparse<TestState64<int, float, char>>();
			world.Sparse<TestState64<long, byte, ulong>>();
			world.Sparse<TestState64<short, decimal, bool>>();
			world.Sparse<TestState64<ushort, double, int>>();

			return world;
		}

		public static World FillWorldWith50Tags(this World world)
		{
			world.Sparse<TestTag>();
			world.Sparse<TestTag<int, int, int>>();
			world.Sparse<TestTag<long, long, long>>();
			world.Sparse<TestTag<double, int, long>>();
			world.Sparse<TestTag<long, short, byte>>();
			world.Sparse<TestTag<short, ushort, int>>();
			world.Sparse<TestTag<ushort, ulong, float>>();
			world.Sparse<TestTag<ulong, double, decimal>>();
			world.Sparse<TestTag<decimal, char, byte>>();
			world.Sparse<TestTag<char, bool, int>>();
			world.Sparse<TestTag<bool, byte, sbyte>>();
			world.Sparse<TestTag<sbyte, float, ushort>>();
			world.Sparse<TestTag<int, double, bool>>();
			world.Sparse<TestTag<double, decimal, char>>();
			world.Sparse<TestTag<long, bool, float>>();
			world.Sparse<TestTag<short, int, ulong>>();
			world.Sparse<TestTag<ushort, sbyte, decimal>>();
			world.Sparse<TestTag<ulong, char, double>>();
			world.Sparse<TestTag<decimal, bool, short>>();
			world.Sparse<TestTag<char, int, byte>>();
			world.Sparse<TestTag<bool, ulong, sbyte>>();
			world.Sparse<TestTag<sbyte, short, ushort>>();
			world.Sparse<TestTag<float, ulong, int>>();
			world.Sparse<TestTag<double, sbyte, long>>();
			world.Sparse<TestTag<int, char, decimal>>();
			world.Sparse<TestTag<long, decimal, bool>>();
			world.Sparse<TestTag<short, double, byte>>();
			world.Sparse<TestTag<ushort, float, char>>();
			world.Sparse<TestTag<ulong, int, bool>>();
			world.Sparse<TestTag<decimal, short, ulong>>();
			world.Sparse<TestTag<char, ushort, float>>();
			world.Sparse<TestTag<bool, double, sbyte>>();
			world.Sparse<TestTag<sbyte, long, ushort>>();
			world.Sparse<TestTag<float, decimal, char>>();
			world.Sparse<TestTag<double, byte, int>>();
			world.Sparse<TestTag<int, sbyte, ulong>>();
			world.Sparse<TestTag<long, ushort, decimal>>();
			world.Sparse<TestTag<short, char, double>>();
			world.Sparse<TestTag<ushort, bool, float>>();
			world.Sparse<TestTag<ulong, byte, short>>();
			world.Sparse<TestTag<decimal, int, char>>();
			world.Sparse<TestTag<char, ulong, bool>>();
			world.Sparse<TestTag<bool, short, double>>();
			world.Sparse<TestTag<sbyte, decimal, ushort>>();
			world.Sparse<TestTag<float, bool, byte>>();
			world.Sparse<TestTag<double, ushort, sbyte>>();
			world.Sparse<TestTag<int, float, char>>();
			world.Sparse<TestTag<long, byte, ulong>>();
			world.Sparse<TestTag<short, decimal, bool>>();
			world.Sparse<TestTag<ushort, double, int>>();

			return world;
		}

		public static World FillWorldWithSingleComponent(this World world)
		{
			world.Sparse<TestState64>();
			return world;
		}

		public static World FillWorldWithNonOwningGroup<TInclude, TExclude>(this World world)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			world.Group<TInclude, TExclude>();
			return world;
		}

		public static World FillWorldWithNonOwningGroup<TInclude>(this World world)
			where TInclude : IIncludeSelector, new()
		{
			world.Group<TInclude>();
			return world;
		}

		public static World FillWorldWithSingleComponent(this World world, int entitiesAmount)
		{
			while (entitiesAmount != 0)
			{
				world.Create<TestState64>();
				entitiesAmount -= 1;
			}

			return world;
		}

		public static World FillWorldWith50Components(this World world, int entitiesAmount)
		{
			while (entitiesAmount != 0)
			{
				// 50 different components.
				var id = world.Create();
				world.Add<TestState64>(id);
				world.Add<TestState64_2>(id);
				world.Add<TestState64_3>(id);
				world.Add<TestState64<double, int, long>>(id);
				world.Add<TestState64<long, short, byte>>(id);
				world.Add<TestState64<short, ushort, int>>(id);
				world.Add<TestState64<ushort, ulong, float>>(id);
				world.Add<TestState64<ulong, double, decimal>>(id);
				world.Add<TestState64<decimal, char, byte>>(id);
				world.Add<TestState64<char, bool, int>>(id);
				world.Add<TestState64<bool, byte, sbyte>>(id);
				world.Add<TestState64<sbyte, float, ushort>>(id);
				world.Add<TestState64<int, double, bool>>(id);
				world.Add<TestState64<double, decimal, char>>(id);
				world.Add<TestState64<long, bool, float>>(id);
				world.Add<TestState64<short, int, ulong>>(id);
				world.Add<TestState64<ushort, sbyte, decimal>>(id);
				world.Add<TestState64<ulong, char, double>>(id);
				world.Add<TestState64<decimal, bool, short>>(id);
				world.Add<TestState64<char, int, byte>>(id);
				world.Add<TestState64<bool, ulong, sbyte>>(id);
				world.Add<TestState64<sbyte, short, ushort>>(id);
				world.Add<TestState64<float, ulong, int>>(id);
				world.Add<TestState64<double, sbyte, long>>(id);
				world.Add<TestState64<int, char, decimal>>(id);
				world.Add<TestState64<long, decimal, bool>>(id);
				world.Add<TestState64<short, double, byte>>(id);
				world.Add<TestState64<ushort, float, char>>(id);
				world.Add<TestState64<ulong, int, bool>>(id);
				world.Add<TestState64<decimal, short, ulong>>(id);
				world.Add<TestState64<char, ushort, float>>(id);
				world.Add<TestState64<bool, double, sbyte>>(id);
				world.Add<TestState64<sbyte, long, ushort>>(id);
				world.Add<TestState64<float, decimal, char>>(id);
				world.Add<TestState64<double, byte, int>>(id);
				world.Add<TestState64<int, sbyte, ulong>>(id);
				world.Add<TestState64<long, ushort, decimal>>(id);
				world.Add<TestState64<short, char, double>>(id);
				world.Add<TestState64<ushort, bool, float>>(id);
				world.Add<TestState64<ulong, byte, short>>(id);
				world.Add<TestState64<decimal, int, char>>(id);
				world.Add<TestState64<char, ulong, bool>>(id);
				world.Add<TestState64<bool, short, double>>(id);
				world.Add<TestState64<sbyte, decimal, ushort>>(id);
				world.Add<TestState64<float, bool, byte>>(id);
				world.Add<TestState64<double, ushort, sbyte>>(id);
				world.Add<TestState64<int, float, char>>(id);
				world.Add<TestState64<long, byte, ulong>>(id);
				world.Add<TestState64<short, decimal, bool>>(id);
				world.Add<TestState64<ushort, double, int>>(id);
				entitiesAmount -= 1;
			}

			return world;
		}

		public static World FillWorldWith50Tags(this World world, int entitiesAmount)
		{
			while (entitiesAmount != 0)
			{
				// 50 different tags.
				var id = world.Create();
				world.Add<TestTag>(id);
				world.Add<TestTag<int, int, int>>(id);
				world.Add<TestTag<long, long, long>>(id);
				world.Add<TestTag<double, int, long>>(id);
				world.Add<TestTag<long, short, byte>>(id);
				world.Add<TestTag<short, ushort, int>>(id);
				world.Add<TestTag<ushort, ulong, float>>(id);
				world.Add<TestTag<ulong, double, decimal>>(id);
				world.Add<TestTag<decimal, char, byte>>(id);
				world.Add<TestTag<char, bool, int>>(id);
				world.Add<TestTag<bool, byte, sbyte>>(id);
				world.Add<TestTag<sbyte, float, ushort>>(id);
				world.Add<TestTag<int, double, bool>>(id);
				world.Add<TestTag<double, decimal, char>>(id);
				world.Add<TestTag<long, bool, float>>(id);
				world.Add<TestTag<short, int, ulong>>(id);
				world.Add<TestTag<ushort, sbyte, decimal>>(id);
				world.Add<TestTag<ulong, char, double>>(id);
				world.Add<TestTag<decimal, bool, short>>(id);
				world.Add<TestTag<char, int, byte>>(id);
				world.Add<TestTag<bool, ulong, sbyte>>(id);
				world.Add<TestTag<sbyte, short, ushort>>(id);
				world.Add<TestTag<float, ulong, int>>(id);
				world.Add<TestTag<double, sbyte, long>>(id);
				world.Add<TestTag<int, char, decimal>>(id);
				world.Add<TestTag<long, decimal, bool>>(id);
				world.Add<TestTag<short, double, byte>>(id);
				world.Add<TestTag<ushort, float, char>>(id);
				world.Add<TestTag<ulong, int, bool>>(id);
				world.Add<TestTag<decimal, short, ulong>>(id);
				world.Add<TestTag<char, ushort, float>>(id);
				world.Add<TestTag<bool, double, sbyte>>(id);
				world.Add<TestTag<sbyte, long, ushort>>(id);
				world.Add<TestTag<float, decimal, char>>(id);
				world.Add<TestTag<double, byte, int>>(id);
				world.Add<TestTag<int, sbyte, ulong>>(id);
				world.Add<TestTag<long, ushort, decimal>>(id);
				world.Add<TestTag<short, char, double>>(id);
				world.Add<TestTag<ushort, bool, float>>(id);
				world.Add<TestTag<ulong, byte, short>>(id);
				world.Add<TestTag<decimal, int, char>>(id);
				world.Add<TestTag<char, ulong, bool>>(id);
				world.Add<TestTag<bool, short, double>>(id);
				world.Add<TestTag<sbyte, decimal, ushort>>(id);
				world.Add<TestTag<float, bool, byte>>(id);
				world.Add<TestTag<double, ushort, sbyte>>(id);
				world.Add<TestTag<int, float, char>>(id);
				world.Add<TestTag<long, byte, ulong>>(id);
				world.Add<TestTag<short, decimal, bool>>(id);
				world.Add<TestTag<ushort, double, int>>(id);
				entitiesAmount -= 1;
			}

			return world;
		}
	}
}

namespace Massive.PerformanceTests
{
	public static class WorldTestUtils
	{
		public static World FillWorldWith50Components(this World world)
		{
			world.Set<TestState64>();
			world.Set<TestState64_2>();
			world.Set<TestState64_3>();
			world.Set<TestState64<double, int, long>>();
			world.Set<TestState64<long, short, byte>>();
			world.Set<TestState64<short, ushort, int>>();
			world.Set<TestState64<ushort, ulong, float>>();
			world.Set<TestState64<ulong, double, decimal>>();
			world.Set<TestState64<decimal, char, byte>>();
			world.Set<TestState64<char, bool, int>>();
			world.Set<TestState64<bool, byte, sbyte>>();
			world.Set<TestState64<sbyte, float, ushort>>();
			world.Set<TestState64<int, double, bool>>();
			world.Set<TestState64<double, decimal, char>>();
			world.Set<TestState64<long, bool, float>>();
			world.Set<TestState64<short, int, ulong>>();
			world.Set<TestState64<ushort, sbyte, decimal>>();
			world.Set<TestState64<ulong, char, double>>();
			world.Set<TestState64<decimal, bool, short>>();
			world.Set<TestState64<char, int, byte>>();
			world.Set<TestState64<bool, ulong, sbyte>>();
			world.Set<TestState64<sbyte, short, ushort>>();
			world.Set<TestState64<float, ulong, int>>();
			world.Set<TestState64<double, sbyte, long>>();
			world.Set<TestState64<int, char, decimal>>();
			world.Set<TestState64<long, decimal, bool>>();
			world.Set<TestState64<short, double, byte>>();
			world.Set<TestState64<ushort, float, char>>();
			world.Set<TestState64<ulong, int, bool>>();
			world.Set<TestState64<decimal, short, ulong>>();
			world.Set<TestState64<char, ushort, float>>();
			world.Set<TestState64<bool, double, sbyte>>();
			world.Set<TestState64<sbyte, long, ushort>>();
			world.Set<TestState64<float, decimal, char>>();
			world.Set<TestState64<double, byte, int>>();
			world.Set<TestState64<int, sbyte, ulong>>();
			world.Set<TestState64<long, ushort, decimal>>();
			world.Set<TestState64<short, char, double>>();
			world.Set<TestState64<ushort, bool, float>>();
			world.Set<TestState64<ulong, byte, short>>();
			world.Set<TestState64<decimal, int, char>>();
			world.Set<TestState64<char, ulong, bool>>();
			world.Set<TestState64<bool, short, double>>();
			world.Set<TestState64<sbyte, decimal, ushort>>();
			world.Set<TestState64<float, bool, byte>>();
			world.Set<TestState64<double, ushort, sbyte>>();
			world.Set<TestState64<int, float, char>>();
			world.Set<TestState64<long, byte, ulong>>();
			world.Set<TestState64<short, decimal, bool>>();
			world.Set<TestState64<ushort, double, int>>();

			return world;
		}

		public static World FillWorldWith50Tags(this World world)
		{
			world.Set<TestTag>();
			world.Set<TestTag<int, int, int>>();
			world.Set<TestTag<long, long, long>>();
			world.Set<TestTag<double, int, long>>();
			world.Set<TestTag<long, short, byte>>();
			world.Set<TestTag<short, ushort, int>>();
			world.Set<TestTag<ushort, ulong, float>>();
			world.Set<TestTag<ulong, double, decimal>>();
			world.Set<TestTag<decimal, char, byte>>();
			world.Set<TestTag<char, bool, int>>();
			world.Set<TestTag<bool, byte, sbyte>>();
			world.Set<TestTag<sbyte, float, ushort>>();
			world.Set<TestTag<int, double, bool>>();
			world.Set<TestTag<double, decimal, char>>();
			world.Set<TestTag<long, bool, float>>();
			world.Set<TestTag<short, int, ulong>>();
			world.Set<TestTag<ushort, sbyte, decimal>>();
			world.Set<TestTag<ulong, char, double>>();
			world.Set<TestTag<decimal, bool, short>>();
			world.Set<TestTag<char, int, byte>>();
			world.Set<TestTag<bool, ulong, sbyte>>();
			world.Set<TestTag<sbyte, short, ushort>>();
			world.Set<TestTag<float, ulong, int>>();
			world.Set<TestTag<double, sbyte, long>>();
			world.Set<TestTag<int, char, decimal>>();
			world.Set<TestTag<long, decimal, bool>>();
			world.Set<TestTag<short, double, byte>>();
			world.Set<TestTag<ushort, float, char>>();
			world.Set<TestTag<ulong, int, bool>>();
			world.Set<TestTag<decimal, short, ulong>>();
			world.Set<TestTag<char, ushort, float>>();
			world.Set<TestTag<bool, double, sbyte>>();
			world.Set<TestTag<sbyte, long, ushort>>();
			world.Set<TestTag<float, decimal, char>>();
			world.Set<TestTag<double, byte, int>>();
			world.Set<TestTag<int, sbyte, ulong>>();
			world.Set<TestTag<long, ushort, decimal>>();
			world.Set<TestTag<short, char, double>>();
			world.Set<TestTag<ushort, bool, float>>();
			world.Set<TestTag<ulong, byte, short>>();
			world.Set<TestTag<decimal, int, char>>();
			world.Set<TestTag<char, ulong, bool>>();
			world.Set<TestTag<bool, short, double>>();
			world.Set<TestTag<sbyte, decimal, ushort>>();
			world.Set<TestTag<float, bool, byte>>();
			world.Set<TestTag<double, ushort, sbyte>>();
			world.Set<TestTag<int, float, char>>();
			world.Set<TestTag<long, byte, ulong>>();
			world.Set<TestTag<short, decimal, bool>>();
			world.Set<TestTag<ushort, double, int>>();

			return world;
		}

		public static World FillWorldWithSingleComponent(this World world)
		{
			world.Set<TestState64>();
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
				world.Assign<TestState64>(id);
				world.Assign<TestState64_2>(id);
				world.Assign<TestState64_3>(id);
				world.Assign<TestState64<double, int, long>>(id);
				world.Assign<TestState64<long, short, byte>>(id);
				world.Assign<TestState64<short, ushort, int>>(id);
				world.Assign<TestState64<ushort, ulong, float>>(id);
				world.Assign<TestState64<ulong, double, decimal>>(id);
				world.Assign<TestState64<decimal, char, byte>>(id);
				world.Assign<TestState64<char, bool, int>>(id);
				world.Assign<TestState64<bool, byte, sbyte>>(id);
				world.Assign<TestState64<sbyte, float, ushort>>(id);
				world.Assign<TestState64<int, double, bool>>(id);
				world.Assign<TestState64<double, decimal, char>>(id);
				world.Assign<TestState64<long, bool, float>>(id);
				world.Assign<TestState64<short, int, ulong>>(id);
				world.Assign<TestState64<ushort, sbyte, decimal>>(id);
				world.Assign<TestState64<ulong, char, double>>(id);
				world.Assign<TestState64<decimal, bool, short>>(id);
				world.Assign<TestState64<char, int, byte>>(id);
				world.Assign<TestState64<bool, ulong, sbyte>>(id);
				world.Assign<TestState64<sbyte, short, ushort>>(id);
				world.Assign<TestState64<float, ulong, int>>(id);
				world.Assign<TestState64<double, sbyte, long>>(id);
				world.Assign<TestState64<int, char, decimal>>(id);
				world.Assign<TestState64<long, decimal, bool>>(id);
				world.Assign<TestState64<short, double, byte>>(id);
				world.Assign<TestState64<ushort, float, char>>(id);
				world.Assign<TestState64<ulong, int, bool>>(id);
				world.Assign<TestState64<decimal, short, ulong>>(id);
				world.Assign<TestState64<char, ushort, float>>(id);
				world.Assign<TestState64<bool, double, sbyte>>(id);
				world.Assign<TestState64<sbyte, long, ushort>>(id);
				world.Assign<TestState64<float, decimal, char>>(id);
				world.Assign<TestState64<double, byte, int>>(id);
				world.Assign<TestState64<int, sbyte, ulong>>(id);
				world.Assign<TestState64<long, ushort, decimal>>(id);
				world.Assign<TestState64<short, char, double>>(id);
				world.Assign<TestState64<ushort, bool, float>>(id);
				world.Assign<TestState64<ulong, byte, short>>(id);
				world.Assign<TestState64<decimal, int, char>>(id);
				world.Assign<TestState64<char, ulong, bool>>(id);
				world.Assign<TestState64<bool, short, double>>(id);
				world.Assign<TestState64<sbyte, decimal, ushort>>(id);
				world.Assign<TestState64<float, bool, byte>>(id);
				world.Assign<TestState64<double, ushort, sbyte>>(id);
				world.Assign<TestState64<int, float, char>>(id);
				world.Assign<TestState64<long, byte, ulong>>(id);
				world.Assign<TestState64<short, decimal, bool>>(id);
				world.Assign<TestState64<ushort, double, int>>(id);
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
				world.Assign<TestTag>(id);
				world.Assign<TestTag<int, int, int>>(id);
				world.Assign<TestTag<long, long, long>>(id);
				world.Assign<TestTag<double, int, long>>(id);
				world.Assign<TestTag<long, short, byte>>(id);
				world.Assign<TestTag<short, ushort, int>>(id);
				world.Assign<TestTag<ushort, ulong, float>>(id);
				world.Assign<TestTag<ulong, double, decimal>>(id);
				world.Assign<TestTag<decimal, char, byte>>(id);
				world.Assign<TestTag<char, bool, int>>(id);
				world.Assign<TestTag<bool, byte, sbyte>>(id);
				world.Assign<TestTag<sbyte, float, ushort>>(id);
				world.Assign<TestTag<int, double, bool>>(id);
				world.Assign<TestTag<double, decimal, char>>(id);
				world.Assign<TestTag<long, bool, float>>(id);
				world.Assign<TestTag<short, int, ulong>>(id);
				world.Assign<TestTag<ushort, sbyte, decimal>>(id);
				world.Assign<TestTag<ulong, char, double>>(id);
				world.Assign<TestTag<decimal, bool, short>>(id);
				world.Assign<TestTag<char, int, byte>>(id);
				world.Assign<TestTag<bool, ulong, sbyte>>(id);
				world.Assign<TestTag<sbyte, short, ushort>>(id);
				world.Assign<TestTag<float, ulong, int>>(id);
				world.Assign<TestTag<double, sbyte, long>>(id);
				world.Assign<TestTag<int, char, decimal>>(id);
				world.Assign<TestTag<long, decimal, bool>>(id);
				world.Assign<TestTag<short, double, byte>>(id);
				world.Assign<TestTag<ushort, float, char>>(id);
				world.Assign<TestTag<ulong, int, bool>>(id);
				world.Assign<TestTag<decimal, short, ulong>>(id);
				world.Assign<TestTag<char, ushort, float>>(id);
				world.Assign<TestTag<bool, double, sbyte>>(id);
				world.Assign<TestTag<sbyte, long, ushort>>(id);
				world.Assign<TestTag<float, decimal, char>>(id);
				world.Assign<TestTag<double, byte, int>>(id);
				world.Assign<TestTag<int, sbyte, ulong>>(id);
				world.Assign<TestTag<long, ushort, decimal>>(id);
				world.Assign<TestTag<short, char, double>>(id);
				world.Assign<TestTag<ushort, bool, float>>(id);
				world.Assign<TestTag<ulong, byte, short>>(id);
				world.Assign<TestTag<decimal, int, char>>(id);
				world.Assign<TestTag<char, ulong, bool>>(id);
				world.Assign<TestTag<bool, short, double>>(id);
				world.Assign<TestTag<sbyte, decimal, ushort>>(id);
				world.Assign<TestTag<float, bool, byte>>(id);
				world.Assign<TestTag<double, ushort, sbyte>>(id);
				world.Assign<TestTag<int, float, char>>(id);
				world.Assign<TestTag<long, byte, ulong>>(id);
				world.Assign<TestTag<short, decimal, bool>>(id);
				world.Assign<TestTag<ushort, double, int>>(id);
				entitiesAmount -= 1;
			}

			return world;
		}
	}
}

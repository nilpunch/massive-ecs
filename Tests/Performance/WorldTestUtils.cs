namespace Massive.PerformanceTests
{
	public static class WorldTestUtils
	{
		public static World FillWorldWith50Components(this World world)
		{
			world.BitSet<TestState64>();
			world.BitSet<TestState64_2>();
			world.BitSet<TestState64_3>();
			world.BitSet<TestState64<double, int, long>>();
			world.BitSet<TestState64<long, short, byte>>();
			world.BitSet<TestState64<short, ushort, int>>();
			world.BitSet<TestState64<ushort, ulong, float>>();
			world.BitSet<TestState64<ulong, double, decimal>>();
			world.BitSet<TestState64<decimal, char, byte>>();
			world.BitSet<TestState64<char, bool, int>>();
			world.BitSet<TestState64<bool, byte, sbyte>>();
			world.BitSet<TestState64<sbyte, float, ushort>>();
			world.BitSet<TestState64<int, double, bool>>();
			world.BitSet<TestState64<double, decimal, char>>();
			world.BitSet<TestState64<long, bool, float>>();
			world.BitSet<TestState64<short, int, ulong>>();
			world.BitSet<TestState64<ushort, sbyte, decimal>>();
			world.BitSet<TestState64<ulong, char, double>>();
			world.BitSet<TestState64<decimal, bool, short>>();
			world.BitSet<TestState64<char, int, byte>>();
			world.BitSet<TestState64<bool, ulong, sbyte>>();
			world.BitSet<TestState64<sbyte, short, ushort>>();
			world.BitSet<TestState64<float, ulong, int>>();
			world.BitSet<TestState64<double, sbyte, long>>();
			world.BitSet<TestState64<int, char, decimal>>();
			world.BitSet<TestState64<long, decimal, bool>>();
			world.BitSet<TestState64<short, double, byte>>();
			world.BitSet<TestState64<ushort, float, char>>();
			world.BitSet<TestState64<ulong, int, bool>>();
			world.BitSet<TestState64<decimal, short, ulong>>();
			world.BitSet<TestState64<char, ushort, float>>();
			world.BitSet<TestState64<bool, double, sbyte>>();
			world.BitSet<TestState64<sbyte, long, ushort>>();
			world.BitSet<TestState64<float, decimal, char>>();
			world.BitSet<TestState64<double, byte, int>>();
			world.BitSet<TestState64<int, sbyte, ulong>>();
			world.BitSet<TestState64<long, ushort, decimal>>();
			world.BitSet<TestState64<short, char, double>>();
			world.BitSet<TestState64<ushort, bool, float>>();
			world.BitSet<TestState64<ulong, byte, short>>();
			world.BitSet<TestState64<decimal, int, char>>();
			world.BitSet<TestState64<char, ulong, bool>>();
			world.BitSet<TestState64<bool, short, double>>();
			world.BitSet<TestState64<sbyte, decimal, ushort>>();
			world.BitSet<TestState64<float, bool, byte>>();
			world.BitSet<TestState64<double, ushort, sbyte>>();
			world.BitSet<TestState64<int, float, char>>();
			world.BitSet<TestState64<long, byte, ulong>>();
			world.BitSet<TestState64<short, decimal, bool>>();
			world.BitSet<TestState64<ushort, double, int>>();

			return world;
		}

		public static World FillWorldWith50Tags(this World world)
		{
			world.BitSet<TestTag>();
			world.BitSet<TestTag<int, int, int>>();
			world.BitSet<TestTag<long, long, long>>();
			world.BitSet<TestTag<double, int, long>>();
			world.BitSet<TestTag<long, short, byte>>();
			world.BitSet<TestTag<short, ushort, int>>();
			world.BitSet<TestTag<ushort, ulong, float>>();
			world.BitSet<TestTag<ulong, double, decimal>>();
			world.BitSet<TestTag<decimal, char, byte>>();
			world.BitSet<TestTag<char, bool, int>>();
			world.BitSet<TestTag<bool, byte, sbyte>>();
			world.BitSet<TestTag<sbyte, float, ushort>>();
			world.BitSet<TestTag<int, double, bool>>();
			world.BitSet<TestTag<double, decimal, char>>();
			world.BitSet<TestTag<long, bool, float>>();
			world.BitSet<TestTag<short, int, ulong>>();
			world.BitSet<TestTag<ushort, sbyte, decimal>>();
			world.BitSet<TestTag<ulong, char, double>>();
			world.BitSet<TestTag<decimal, bool, short>>();
			world.BitSet<TestTag<char, int, byte>>();
			world.BitSet<TestTag<bool, ulong, sbyte>>();
			world.BitSet<TestTag<sbyte, short, ushort>>();
			world.BitSet<TestTag<float, ulong, int>>();
			world.BitSet<TestTag<double, sbyte, long>>();
			world.BitSet<TestTag<int, char, decimal>>();
			world.BitSet<TestTag<long, decimal, bool>>();
			world.BitSet<TestTag<short, double, byte>>();
			world.BitSet<TestTag<ushort, float, char>>();
			world.BitSet<TestTag<ulong, int, bool>>();
			world.BitSet<TestTag<decimal, short, ulong>>();
			world.BitSet<TestTag<char, ushort, float>>();
			world.BitSet<TestTag<bool, double, sbyte>>();
			world.BitSet<TestTag<sbyte, long, ushort>>();
			world.BitSet<TestTag<float, decimal, char>>();
			world.BitSet<TestTag<double, byte, int>>();
			world.BitSet<TestTag<int, sbyte, ulong>>();
			world.BitSet<TestTag<long, ushort, decimal>>();
			world.BitSet<TestTag<short, char, double>>();
			world.BitSet<TestTag<ushort, bool, float>>();
			world.BitSet<TestTag<ulong, byte, short>>();
			world.BitSet<TestTag<decimal, int, char>>();
			world.BitSet<TestTag<char, ulong, bool>>();
			world.BitSet<TestTag<bool, short, double>>();
			world.BitSet<TestTag<sbyte, decimal, ushort>>();
			world.BitSet<TestTag<float, bool, byte>>();
			world.BitSet<TestTag<double, ushort, sbyte>>();
			world.BitSet<TestTag<int, float, char>>();
			world.BitSet<TestTag<long, byte, ulong>>();
			world.BitSet<TestTag<short, decimal, bool>>();
			world.BitSet<TestTag<ushort, double, int>>();

			return world;
		}

		public static World FillWorldWithSingleComponent(this World world)
		{
			world.BitSet<TestState64>();
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

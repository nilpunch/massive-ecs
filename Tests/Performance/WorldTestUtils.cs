namespace Massive.PerformanceTests
{
	public static class WorldTestUtils
	{
		public static World FillWorldWith50Components(this World world)
		{
			world.SparseSet<TestState64>();
			world.SparseSet<TestState64_2>();
			world.SparseSet<TestState64_3>();
			world.SparseSet<TestState64<double, int, long>>();
			world.SparseSet<TestState64<long, short, byte>>();
			world.SparseSet<TestState64<short, ushort, int>>();
			world.SparseSet<TestState64<ushort, ulong, float>>();
			world.SparseSet<TestState64<ulong, double, decimal>>();
			world.SparseSet<TestState64<decimal, char, byte>>();
			world.SparseSet<TestState64<char, bool, int>>();
			world.SparseSet<TestState64<bool, byte, sbyte>>();
			world.SparseSet<TestState64<sbyte, float, ushort>>();
			world.SparseSet<TestState64<int, double, bool>>();
			world.SparseSet<TestState64<double, decimal, char>>();
			world.SparseSet<TestState64<long, bool, float>>();
			world.SparseSet<TestState64<short, int, ulong>>();
			world.SparseSet<TestState64<ushort, sbyte, decimal>>();
			world.SparseSet<TestState64<ulong, char, double>>();
			world.SparseSet<TestState64<decimal, bool, short>>();
			world.SparseSet<TestState64<char, int, byte>>();
			world.SparseSet<TestState64<bool, ulong, sbyte>>();
			world.SparseSet<TestState64<sbyte, short, ushort>>();
			world.SparseSet<TestState64<float, ulong, int>>();
			world.SparseSet<TestState64<double, sbyte, long>>();
			world.SparseSet<TestState64<int, char, decimal>>();
			world.SparseSet<TestState64<long, decimal, bool>>();
			world.SparseSet<TestState64<short, double, byte>>();
			world.SparseSet<TestState64<ushort, float, char>>();
			world.SparseSet<TestState64<ulong, int, bool>>();
			world.SparseSet<TestState64<decimal, short, ulong>>();
			world.SparseSet<TestState64<char, ushort, float>>();
			world.SparseSet<TestState64<bool, double, sbyte>>();
			world.SparseSet<TestState64<sbyte, long, ushort>>();
			world.SparseSet<TestState64<float, decimal, char>>();
			world.SparseSet<TestState64<double, byte, int>>();
			world.SparseSet<TestState64<int, sbyte, ulong>>();
			world.SparseSet<TestState64<long, ushort, decimal>>();
			world.SparseSet<TestState64<short, char, double>>();
			world.SparseSet<TestState64<ushort, bool, float>>();
			world.SparseSet<TestState64<ulong, byte, short>>();
			world.SparseSet<TestState64<decimal, int, char>>();
			world.SparseSet<TestState64<char, ulong, bool>>();
			world.SparseSet<TestState64<bool, short, double>>();
			world.SparseSet<TestState64<sbyte, decimal, ushort>>();
			world.SparseSet<TestState64<float, bool, byte>>();
			world.SparseSet<TestState64<double, ushort, sbyte>>();
			world.SparseSet<TestState64<int, float, char>>();
			world.SparseSet<TestState64<long, byte, ulong>>();
			world.SparseSet<TestState64<short, decimal, bool>>();
			world.SparseSet<TestState64<ushort, double, int>>();

			return world;
		}

		public static World FillWorldWith50Tags(this World world)
		{
			world.SparseSet<TestTag>();
			world.SparseSet<TestTag<int, int, int>>();
			world.SparseSet<TestTag<long, long, long>>();
			world.SparseSet<TestTag<double, int, long>>();
			world.SparseSet<TestTag<long, short, byte>>();
			world.SparseSet<TestTag<short, ushort, int>>();
			world.SparseSet<TestTag<ushort, ulong, float>>();
			world.SparseSet<TestTag<ulong, double, decimal>>();
			world.SparseSet<TestTag<decimal, char, byte>>();
			world.SparseSet<TestTag<char, bool, int>>();
			world.SparseSet<TestTag<bool, byte, sbyte>>();
			world.SparseSet<TestTag<sbyte, float, ushort>>();
			world.SparseSet<TestTag<int, double, bool>>();
			world.SparseSet<TestTag<double, decimal, char>>();
			world.SparseSet<TestTag<long, bool, float>>();
			world.SparseSet<TestTag<short, int, ulong>>();
			world.SparseSet<TestTag<ushort, sbyte, decimal>>();
			world.SparseSet<TestTag<ulong, char, double>>();
			world.SparseSet<TestTag<decimal, bool, short>>();
			world.SparseSet<TestTag<char, int, byte>>();
			world.SparseSet<TestTag<bool, ulong, sbyte>>();
			world.SparseSet<TestTag<sbyte, short, ushort>>();
			world.SparseSet<TestTag<float, ulong, int>>();
			world.SparseSet<TestTag<double, sbyte, long>>();
			world.SparseSet<TestTag<int, char, decimal>>();
			world.SparseSet<TestTag<long, decimal, bool>>();
			world.SparseSet<TestTag<short, double, byte>>();
			world.SparseSet<TestTag<ushort, float, char>>();
			world.SparseSet<TestTag<ulong, int, bool>>();
			world.SparseSet<TestTag<decimal, short, ulong>>();
			world.SparseSet<TestTag<char, ushort, float>>();
			world.SparseSet<TestTag<bool, double, sbyte>>();
			world.SparseSet<TestTag<sbyte, long, ushort>>();
			world.SparseSet<TestTag<float, decimal, char>>();
			world.SparseSet<TestTag<double, byte, int>>();
			world.SparseSet<TestTag<int, sbyte, ulong>>();
			world.SparseSet<TestTag<long, ushort, decimal>>();
			world.SparseSet<TestTag<short, char, double>>();
			world.SparseSet<TestTag<ushort, bool, float>>();
			world.SparseSet<TestTag<ulong, byte, short>>();
			world.SparseSet<TestTag<decimal, int, char>>();
			world.SparseSet<TestTag<char, ulong, bool>>();
			world.SparseSet<TestTag<bool, short, double>>();
			world.SparseSet<TestTag<sbyte, decimal, ushort>>();
			world.SparseSet<TestTag<float, bool, byte>>();
			world.SparseSet<TestTag<double, ushort, sbyte>>();
			world.SparseSet<TestTag<int, float, char>>();
			world.SparseSet<TestTag<long, byte, ulong>>();
			world.SparseSet<TestTag<short, decimal, bool>>();
			world.SparseSet<TestTag<ushort, double, int>>();

			return world;
		}

		public static World FillWorldWithSingleComponent(this World world)
		{
			world.SparseSet<TestState64>();
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

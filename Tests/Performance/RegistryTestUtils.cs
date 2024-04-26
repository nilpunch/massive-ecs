namespace Massive.PerformanceTests
{
	public static class RegistryTestUtils
	{
		public static IRegistry FillRegistryWithSingleComponent(this IRegistry registry, int entitiesAmount)
		{
			while (entitiesAmount != 0)
			{
				registry.Create<TestState64>();
				entitiesAmount -= 1;
			}

			return registry;
		}

		public static IRegistry FillRegistryWith50Components(this IRegistry registry, int entitiesAmount)
		{
			while (entitiesAmount != 0)
			{
				// 50 different components
				var id = registry.Create();
				registry.Assign<TestState64>(id);
				registry.Assign<TestState64_2>(id);
				registry.Assign<TestState64_3>(id);
				registry.Assign<TestState64<double, int, long>>(id);
				registry.Assign<TestState64<long, short, byte>>(id);
				registry.Assign<TestState64<short, ushort, int>>(id);
				registry.Assign<TestState64<ushort, ulong, float>>(id);
				registry.Assign<TestState64<ulong, double, decimal>>(id);
				registry.Assign<TestState64<decimal, char, byte>>(id);
				registry.Assign<TestState64<char, bool, int>>(id);
				registry.Assign<TestState64<bool, byte, sbyte>>(id);
				registry.Assign<TestState64<sbyte, float, ushort>>(id);
				registry.Assign<TestState64<int, double, bool>>(id);
				registry.Assign<TestState64<double, decimal, char>>(id);
				registry.Assign<TestState64<long, bool, float>>(id);
				registry.Assign<TestState64<short, int, ulong>>(id);
				registry.Assign<TestState64<ushort, sbyte, decimal>>(id);
				registry.Assign<TestState64<ulong, char, double>>(id);
				registry.Assign<TestState64<decimal, bool, short>>(id);
				registry.Assign<TestState64<char, int, byte>>(id);
				registry.Assign<TestState64<bool, ulong, sbyte>>(id);
				registry.Assign<TestState64<sbyte, short, ushort>>(id);
				registry.Assign<TestState64<float, ulong, int>>(id);
				registry.Assign<TestState64<double, sbyte, long>>(id);
				registry.Assign<TestState64<int, char, decimal>>(id);
				registry.Assign<TestState64<long, decimal, bool>>(id);
				registry.Assign<TestState64<short, double, byte>>(id);
				registry.Assign<TestState64<ushort, float, char>>(id);
				registry.Assign<TestState64<ulong, int, bool>>(id);
				registry.Assign<TestState64<decimal, short, ulong>>(id);
				registry.Assign<TestState64<char, ushort, float>>(id);
				registry.Assign<TestState64<bool, double, sbyte>>(id);
				registry.Assign<TestState64<sbyte, long, ushort>>(id);
				registry.Assign<TestState64<float, decimal, char>>(id);
				registry.Assign<TestState64<double, byte, int>>(id);
				registry.Assign<TestState64<int, sbyte, ulong>>(id);
				registry.Assign<TestState64<long, ushort, decimal>>(id);
				registry.Assign<TestState64<short, char, double>>(id);
				registry.Assign<TestState64<ushort, bool, float>>(id);
				registry.Assign<TestState64<ulong, byte, short>>(id);
				registry.Assign<TestState64<decimal, int, char>>(id);
				registry.Assign<TestState64<char, ulong, bool>>(id);
				registry.Assign<TestState64<bool, short, double>>(id);
				registry.Assign<TestState64<sbyte, decimal, ushort>>(id);
				registry.Assign<TestState64<float, bool, byte>>(id);
				registry.Assign<TestState64<double, ushort, sbyte>>(id);
				registry.Assign<TestState64<int, float, char>>(id);
				registry.Assign<TestState64<long, byte, ulong>>(id);
				registry.Assign<TestState64<short, decimal, bool>>(id);
				registry.Assign<TestState64<ushort, double, int>>(id);
				entitiesAmount -= 1;
			}

			return registry;
		}

		public static IRegistry FillRegistryWith50Tags(this IRegistry registry, int entitiesAmount)
		{
			while (entitiesAmount != 0)
			{
				// 50 different tags
				var id = registry.Create();
				registry.Assign<TestTag>(id);
				registry.Assign<TestTag<int, int, int>>(id);
				registry.Assign<TestTag<long, long, long>>(id);
				registry.Assign<TestTag<double, int, long>>(id);
				registry.Assign<TestTag<long, short, byte>>(id);
				registry.Assign<TestTag<short, ushort, int>>(id);
				registry.Assign<TestTag<ushort, ulong, float>>(id);
				registry.Assign<TestTag<ulong, double, decimal>>(id);
				registry.Assign<TestTag<decimal, char, byte>>(id);
				registry.Assign<TestTag<char, bool, int>>(id);
				registry.Assign<TestTag<bool, byte, sbyte>>(id);
				registry.Assign<TestTag<sbyte, float, ushort>>(id);
				registry.Assign<TestTag<int, double, bool>>(id);
				registry.Assign<TestTag<double, decimal, char>>(id);
				registry.Assign<TestTag<long, bool, float>>(id);
				registry.Assign<TestTag<short, int, ulong>>(id);
				registry.Assign<TestTag<ushort, sbyte, decimal>>(id);
				registry.Assign<TestTag<ulong, char, double>>(id);
				registry.Assign<TestTag<decimal, bool, short>>(id);
				registry.Assign<TestTag<char, int, byte>>(id);
				registry.Assign<TestTag<bool, ulong, sbyte>>(id);
				registry.Assign<TestTag<sbyte, short, ushort>>(id);
				registry.Assign<TestTag<float, ulong, int>>(id);
				registry.Assign<TestTag<double, sbyte, long>>(id);
				registry.Assign<TestTag<int, char, decimal>>(id);
				registry.Assign<TestTag<long, decimal, bool>>(id);
				registry.Assign<TestTag<short, double, byte>>(id);
				registry.Assign<TestTag<ushort, float, char>>(id);
				registry.Assign<TestTag<ulong, int, bool>>(id);
				registry.Assign<TestTag<decimal, short, ulong>>(id);
				registry.Assign<TestTag<char, ushort, float>>(id);
				registry.Assign<TestTag<bool, double, sbyte>>(id);
				registry.Assign<TestTag<sbyte, long, ushort>>(id);
				registry.Assign<TestTag<float, decimal, char>>(id);
				registry.Assign<TestTag<double, byte, int>>(id);
				registry.Assign<TestTag<int, sbyte, ulong>>(id);
				registry.Assign<TestTag<long, ushort, decimal>>(id);
				registry.Assign<TestTag<short, char, double>>(id);
				registry.Assign<TestTag<ushort, bool, float>>(id);
				registry.Assign<TestTag<ulong, byte, short>>(id);
				registry.Assign<TestTag<decimal, int, char>>(id);
				registry.Assign<TestTag<char, ulong, bool>>(id);
				registry.Assign<TestTag<bool, short, double>>(id);
				registry.Assign<TestTag<sbyte, decimal, ushort>>(id);
				registry.Assign<TestTag<float, bool, byte>>(id);
				registry.Assign<TestTag<double, ushort, sbyte>>(id);
				registry.Assign<TestTag<int, float, char>>(id);
				registry.Assign<TestTag<long, byte, ulong>>(id);
				registry.Assign<TestTag<short, decimal, bool>>(id);
				registry.Assign<TestTag<ushort, double, int>>(id);
				entitiesAmount -= 1;
			}

			return registry;
		}
	}
}

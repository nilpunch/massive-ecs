namespace Massive.PerformanceTests
{
	public static class RegistryTestUtils
	{
		public static IRegistry FillRegistryWithSingleComponent(this IRegistry registry)
		{
			while (registry.Entities.CanCreateAmount != 0)
			{
				registry.Create<TestState64>();
			}

			return registry;
		}

		public static IRegistry FillRegistryWith50Components(this IRegistry registry)
		{
			while (registry.Entities.CanCreateAmount != 0)
			{
				// 50 different components
				var id = registry.Create();
				registry.Add<TestState64>(id);
				registry.Add<TestState64_2>(id);
				registry.Add<TestState64_3>(id);
				registry.Add<TestState64<double, int, long>>(id);
				registry.Add<TestState64<long, short, byte>>(id);
				registry.Add<TestState64<short, ushort, int>>(id);
				registry.Add<TestState64<ushort, ulong, float>>(id);
				registry.Add<TestState64<ulong, double, decimal>>(id);
				registry.Add<TestState64<decimal, char, byte>>(id);
				registry.Add<TestState64<char, bool, int>>(id);
				registry.Add<TestState64<bool, byte, sbyte>>(id);
				registry.Add<TestState64<sbyte, float, ushort>>(id);
				registry.Add<TestState64<int, double, bool>>(id);
				registry.Add<TestState64<double, decimal, char>>(id);
				registry.Add<TestState64<long, bool, float>>(id);
				registry.Add<TestState64<short, int, ulong>>(id);
				registry.Add<TestState64<ushort, sbyte, decimal>>(id);
				registry.Add<TestState64<ulong, char, double>>(id);
				registry.Add<TestState64<decimal, bool, short>>(id);
				registry.Add<TestState64<char, int, byte>>(id);
				registry.Add<TestState64<bool, ulong, sbyte>>(id);
				registry.Add<TestState64<sbyte, short, ushort>>(id);
				registry.Add<TestState64<float, ulong, int>>(id);
				registry.Add<TestState64<double, sbyte, long>>(id);
				registry.Add<TestState64<int, char, decimal>>(id);
				registry.Add<TestState64<long, decimal, bool>>(id);
				registry.Add<TestState64<short, double, byte>>(id);
				registry.Add<TestState64<ushort, float, char>>(id);
				registry.Add<TestState64<ulong, int, bool>>(id);
				registry.Add<TestState64<decimal, short, ulong>>(id);
				registry.Add<TestState64<char, ushort, float>>(id);
				registry.Add<TestState64<bool, double, sbyte>>(id);
				registry.Add<TestState64<sbyte, long, ushort>>(id);
				registry.Add<TestState64<float, decimal, char>>(id);
				registry.Add<TestState64<double, byte, int>>(id);
				registry.Add<TestState64<int, sbyte, ulong>>(id);
				registry.Add<TestState64<long, ushort, decimal>>(id);
				registry.Add<TestState64<short, char, double>>(id);
				registry.Add<TestState64<ushort, bool, float>>(id);
				registry.Add<TestState64<ulong, byte, short>>(id);
				registry.Add<TestState64<decimal, int, char>>(id);
				registry.Add<TestState64<char, ulong, bool>>(id);
				registry.Add<TestState64<bool, short, double>>(id);
				registry.Add<TestState64<sbyte, decimal, ushort>>(id);
				registry.Add<TestState64<float, bool, byte>>(id);
				registry.Add<TestState64<double, ushort, sbyte>>(id);
				registry.Add<TestState64<int, float, char>>(id);
				registry.Add<TestState64<long, byte, ulong>>(id);
				registry.Add<TestState64<short, decimal, bool>>(id);
				registry.Add<TestState64<ushort, double, int>>(id);
			}

			return registry;
		}

		public static IRegistry FillRegistryWith50Tags(this IRegistry registry)
		{
			while (registry.Entities.CanCreateAmount != 0)
			{
				// 50 different tags
				var id = registry.Create();
				registry.Add<TestTag>(id);
				registry.Add<TestTag<int, int, int>>(id);
				registry.Add<TestTag<long, long, long>>(id);
				registry.Add<TestTag<double, int, long>>(id);
				registry.Add<TestTag<long, short, byte>>(id);
				registry.Add<TestTag<short, ushort, int>>(id);
				registry.Add<TestTag<ushort, ulong, float>>(id);
				registry.Add<TestTag<ulong, double, decimal>>(id);
				registry.Add<TestTag<decimal, char, byte>>(id);
				registry.Add<TestTag<char, bool, int>>(id);
				registry.Add<TestTag<bool, byte, sbyte>>(id);
				registry.Add<TestTag<sbyte, float, ushort>>(id);
				registry.Add<TestTag<int, double, bool>>(id);
				registry.Add<TestTag<double, decimal, char>>(id);
				registry.Add<TestTag<long, bool, float>>(id);
				registry.Add<TestTag<short, int, ulong>>(id);
				registry.Add<TestTag<ushort, sbyte, decimal>>(id);
				registry.Add<TestTag<ulong, char, double>>(id);
				registry.Add<TestTag<decimal, bool, short>>(id);
				registry.Add<TestTag<char, int, byte>>(id);
				registry.Add<TestTag<bool, ulong, sbyte>>(id);
				registry.Add<TestTag<sbyte, short, ushort>>(id);
				registry.Add<TestTag<float, ulong, int>>(id);
				registry.Add<TestTag<double, sbyte, long>>(id);
				registry.Add<TestTag<int, char, decimal>>(id);
				registry.Add<TestTag<long, decimal, bool>>(id);
				registry.Add<TestTag<short, double, byte>>(id);
				registry.Add<TestTag<ushort, float, char>>(id);
				registry.Add<TestTag<ulong, int, bool>>(id);
				registry.Add<TestTag<decimal, short, ulong>>(id);
				registry.Add<TestTag<char, ushort, float>>(id);
				registry.Add<TestTag<bool, double, sbyte>>(id);
				registry.Add<TestTag<sbyte, long, ushort>>(id);
				registry.Add<TestTag<float, decimal, char>>(id);
				registry.Add<TestTag<double, byte, int>>(id);
				registry.Add<TestTag<int, sbyte, ulong>>(id);
				registry.Add<TestTag<long, ushort, decimal>>(id);
				registry.Add<TestTag<short, char, double>>(id);
				registry.Add<TestTag<ushort, bool, float>>(id);
				registry.Add<TestTag<ulong, byte, short>>(id);
				registry.Add<TestTag<decimal, int, char>>(id);
				registry.Add<TestTag<char, ulong, bool>>(id);
				registry.Add<TestTag<bool, short, double>>(id);
				registry.Add<TestTag<sbyte, decimal, ushort>>(id);
				registry.Add<TestTag<float, bool, byte>>(id);
				registry.Add<TestTag<double, ushort, sbyte>>(id);
				registry.Add<TestTag<int, float, char>>(id);
				registry.Add<TestTag<long, byte, ulong>>(id);
				registry.Add<TestTag<short, decimal, bool>>(id);
				registry.Add<TestTag<ushort, double, int>>(id);
			}

			return registry;
		}
	}
}
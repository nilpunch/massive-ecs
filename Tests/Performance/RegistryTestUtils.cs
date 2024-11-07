namespace Massive.PerformanceTests
{
	public static class RegistryTestUtils
	{
		public static Registry FillRegistryWith50Components(this Registry registry)
		{
			registry.Set<TestState64>();
			registry.Set<TestState64_2>();
			registry.Set<TestState64_3>();
			registry.Set<TestState64<double, int, long>>();
			registry.Set<TestState64<long, short, byte>>();
			registry.Set<TestState64<short, ushort, int>>();
			registry.Set<TestState64<ushort, ulong, float>>();
			registry.Set<TestState64<ulong, double, decimal>>();
			registry.Set<TestState64<decimal, char, byte>>();
			registry.Set<TestState64<char, bool, int>>();
			registry.Set<TestState64<bool, byte, sbyte>>();
			registry.Set<TestState64<sbyte, float, ushort>>();
			registry.Set<TestState64<int, double, bool>>();
			registry.Set<TestState64<double, decimal, char>>();
			registry.Set<TestState64<long, bool, float>>();
			registry.Set<TestState64<short, int, ulong>>();
			registry.Set<TestState64<ushort, sbyte, decimal>>();
			registry.Set<TestState64<ulong, char, double>>();
			registry.Set<TestState64<decimal, bool, short>>();
			registry.Set<TestState64<char, int, byte>>();
			registry.Set<TestState64<bool, ulong, sbyte>>();
			registry.Set<TestState64<sbyte, short, ushort>>();
			registry.Set<TestState64<float, ulong, int>>();
			registry.Set<TestState64<double, sbyte, long>>();
			registry.Set<TestState64<int, char, decimal>>();
			registry.Set<TestState64<long, decimal, bool>>();
			registry.Set<TestState64<short, double, byte>>();
			registry.Set<TestState64<ushort, float, char>>();
			registry.Set<TestState64<ulong, int, bool>>();
			registry.Set<TestState64<decimal, short, ulong>>();
			registry.Set<TestState64<char, ushort, float>>();
			registry.Set<TestState64<bool, double, sbyte>>();
			registry.Set<TestState64<sbyte, long, ushort>>();
			registry.Set<TestState64<float, decimal, char>>();
			registry.Set<TestState64<double, byte, int>>();
			registry.Set<TestState64<int, sbyte, ulong>>();
			registry.Set<TestState64<long, ushort, decimal>>();
			registry.Set<TestState64<short, char, double>>();
			registry.Set<TestState64<ushort, bool, float>>();
			registry.Set<TestState64<ulong, byte, short>>();
			registry.Set<TestState64<decimal, int, char>>();
			registry.Set<TestState64<char, ulong, bool>>();
			registry.Set<TestState64<bool, short, double>>();
			registry.Set<TestState64<sbyte, decimal, ushort>>();
			registry.Set<TestState64<float, bool, byte>>();
			registry.Set<TestState64<double, ushort, sbyte>>();
			registry.Set<TestState64<int, float, char>>();
			registry.Set<TestState64<long, byte, ulong>>();
			registry.Set<TestState64<short, decimal, bool>>();
			registry.Set<TestState64<ushort, double, int>>();

			return registry;
		}

		public static Registry FillRegistryWith50Tags(this Registry registry)
		{
			registry.Set<TestTag>();
			registry.Set<TestTag<int, int, int>>();
			registry.Set<TestTag<long, long, long>>();
			registry.Set<TestTag<double, int, long>>();
			registry.Set<TestTag<long, short, byte>>();
			registry.Set<TestTag<short, ushort, int>>();
			registry.Set<TestTag<ushort, ulong, float>>();
			registry.Set<TestTag<ulong, double, decimal>>();
			registry.Set<TestTag<decimal, char, byte>>();
			registry.Set<TestTag<char, bool, int>>();
			registry.Set<TestTag<bool, byte, sbyte>>();
			registry.Set<TestTag<sbyte, float, ushort>>();
			registry.Set<TestTag<int, double, bool>>();
			registry.Set<TestTag<double, decimal, char>>();
			registry.Set<TestTag<long, bool, float>>();
			registry.Set<TestTag<short, int, ulong>>();
			registry.Set<TestTag<ushort, sbyte, decimal>>();
			registry.Set<TestTag<ulong, char, double>>();
			registry.Set<TestTag<decimal, bool, short>>();
			registry.Set<TestTag<char, int, byte>>();
			registry.Set<TestTag<bool, ulong, sbyte>>();
			registry.Set<TestTag<sbyte, short, ushort>>();
			registry.Set<TestTag<float, ulong, int>>();
			registry.Set<TestTag<double, sbyte, long>>();
			registry.Set<TestTag<int, char, decimal>>();
			registry.Set<TestTag<long, decimal, bool>>();
			registry.Set<TestTag<short, double, byte>>();
			registry.Set<TestTag<ushort, float, char>>();
			registry.Set<TestTag<ulong, int, bool>>();
			registry.Set<TestTag<decimal, short, ulong>>();
			registry.Set<TestTag<char, ushort, float>>();
			registry.Set<TestTag<bool, double, sbyte>>();
			registry.Set<TestTag<sbyte, long, ushort>>();
			registry.Set<TestTag<float, decimal, char>>();
			registry.Set<TestTag<double, byte, int>>();
			registry.Set<TestTag<int, sbyte, ulong>>();
			registry.Set<TestTag<long, ushort, decimal>>();
			registry.Set<TestTag<short, char, double>>();
			registry.Set<TestTag<ushort, bool, float>>();
			registry.Set<TestTag<ulong, byte, short>>();
			registry.Set<TestTag<decimal, int, char>>();
			registry.Set<TestTag<char, ulong, bool>>();
			registry.Set<TestTag<bool, short, double>>();
			registry.Set<TestTag<sbyte, decimal, ushort>>();
			registry.Set<TestTag<float, bool, byte>>();
			registry.Set<TestTag<double, ushort, sbyte>>();
			registry.Set<TestTag<int, float, char>>();
			registry.Set<TestTag<long, byte, ulong>>();
			registry.Set<TestTag<short, decimal, bool>>();
			registry.Set<TestTag<ushort, double, int>>();

			return registry;
		}

		public static Registry FillRegistryWithSingleComponent(this Registry registry)
		{
			registry.Set<TestState64>();
			return registry;
		}

		public static Registry FillRegistryWithNonOwningGroup<TInclude, TExclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
			where TExclude : IExcludeSelector, new()
		{
			registry.ReactiveFilter<TInclude, TExclude>();
			return registry;
		}

		public static Registry FillRegistryWithNonOwningGroup<TInclude>(this Registry registry)
			where TInclude : IIncludeSelector, new()
		{
			registry.ReactiveFilter<TInclude>();
			return registry;
		}

		public static Registry FillRegistryWithSingleComponent(this Registry registry, int entitiesAmount)
		{
			while (entitiesAmount != 0)
			{
				registry.Create<TestState64>();
				entitiesAmount -= 1;
			}

			return registry;
		}

		public static Registry FillRegistryWith50Components(this Registry registry, int entitiesAmount)
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

		public static Registry FillRegistryWith50Tags(this Registry registry, int entitiesAmount)
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

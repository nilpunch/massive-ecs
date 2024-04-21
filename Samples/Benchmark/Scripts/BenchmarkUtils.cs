namespace Massive.Samples.Benchmark
{
	public static class BenchmarkUtils
	{
		public static MassiveRegistry GetSimplyPackedRegistry(int entitiesCapacity, int framesCapacity)
		{
			MassiveRegistry registry = new MassiveRegistry(entitiesCapacity, framesCapacity + 1);

			for (int i = 0; i < entitiesCapacity; i++)
			{
				registry.Create<TestState>();
			}

			return registry;
		}

		public static MassiveRegistry GetFullyPackedRegistry(int entitiesCapacity, int framesCapacity)
		{
			MassiveRegistry registry = new MassiveRegistry(entitiesCapacity, framesCapacity + 1);

			for (int i = 0; i < entitiesCapacity; i++)
			{
				// 50 different components
				var id = registry.Create();
				registry.Assign<TestState>(id);
				registry.Assign<TestState<float, byte, int>>(id);
				registry.Assign<TestState<int, float, double>>(id);
				registry.Assign<TestState<double, int, long>>(id);
				registry.Assign<TestState<long, short, byte>>(id);
				registry.Assign<TestState<short, ushort, int>>(id);
				registry.Assign<TestState<ushort, ulong, float>>(id);
				registry.Assign<TestState<ulong, double, decimal>>(id);
				registry.Assign<TestState<decimal, char, byte>>(id);
				registry.Assign<TestState<char, bool, int>>(id);
				registry.Assign<TestState<bool, byte, sbyte>>(id);
				registry.Assign<TestState<sbyte, float, ushort>>(id);
				registry.Assign<TestState<int, double, bool>>(id);
				registry.Assign<TestState<double, decimal, char>>(id);
				registry.Assign<TestState<long, bool, float>>(id);
				registry.Assign<TestState<short, int, ulong>>(id);
				registry.Assign<TestState<ushort, sbyte, decimal>>(id);
				registry.Assign<TestState<ulong, char, double>>(id);
				registry.Assign<TestState<decimal, bool, short>>(id);
				registry.Assign<TestState<char, int, byte>>(id);
				registry.Assign<TestState<bool, ulong, sbyte>>(id);
				registry.Assign<TestState<sbyte, short, ushort>>(id);
				registry.Assign<TestState<float, ulong, int>>(id);
				registry.Assign<TestState<double, sbyte, long>>(id);
				registry.Assign<TestState<int, char, decimal>>(id);
				registry.Assign<TestState<long, decimal, bool>>(id);
				registry.Assign<TestState<short, double, byte>>(id);
				registry.Assign<TestState<ushort, float, char>>(id);
				registry.Assign<TestState<ulong, int, bool>>(id);
				registry.Assign<TestState<decimal, short, ulong>>(id);
				registry.Assign<TestState<char, ushort, float>>(id);
				registry.Assign<TestState<bool, double, sbyte>>(id);
				registry.Assign<TestState<sbyte, long, ushort>>(id);
				registry.Assign<TestState<float, decimal, char>>(id);
				registry.Assign<TestState<double, byte, int>>(id);
				registry.Assign<TestState<int, sbyte, ulong>>(id);
				registry.Assign<TestState<long, ushort, decimal>>(id);
				registry.Assign<TestState<short, char, double>>(id);
				registry.Assign<TestState<ushort, bool, float>>(id);
				registry.Assign<TestState<ulong, byte, short>>(id);
				registry.Assign<TestState<decimal, int, char>>(id);
				registry.Assign<TestState<char, ulong, bool>>(id);
				registry.Assign<TestState<bool, short, double>>(id);
				registry.Assign<TestState<sbyte, decimal, ushort>>(id);
				registry.Assign<TestState<float, bool, byte>>(id);
				registry.Assign<TestState<double, ushort, sbyte>>(id);
				registry.Assign<TestState<int, float, char>>(id);
				registry.Assign<TestState<long, byte, ulong>>(id);
				registry.Assign<TestState<short, decimal, bool>>(id);
				registry.Assign<TestState<ushort, double, int>>(id);
			}

			return registry;
		}
	}
}
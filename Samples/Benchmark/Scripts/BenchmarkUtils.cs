namespace Massive.Samples.Benchmark
{
	public static class BenchmarkUtils
	{
		public static MassiveRegistry GetSimplyPackedRegistry(int framesCapacity, int entitiesCapacity)
		{
			MassiveRegistry registry = new MassiveRegistry(framesCapacity + 1, entitiesCapacity);

			for (int i = 0; i < entitiesCapacity; i++)
			{
				registry.Create<TestState>();
			}

			return registry;
		}

		public static MassiveRegistry GetFullyPackedRegistry(int framesCapacity, int entitiesCapacity)
		{
			MassiveRegistry registry = new MassiveRegistry(framesCapacity + 1, entitiesCapacity);

			for (int i = 0; i < entitiesCapacity; i++)
			{
				// 50 different components
				int id = registry.Create();
				registry.Add<TestState>(id);
				registry.Add<TestState<float, byte, int>>(id);
				registry.Add<TestState<int, float, double>>(id);
				registry.Add<TestState<double, int, long>>(id);
				registry.Add<TestState<long, short, byte>>(id);
				registry.Add<TestState<short, ushort, int>>(id);
				registry.Add<TestState<ushort, ulong, float>>(id);
				registry.Add<TestState<ulong, double, decimal>>(id);
				registry.Add<TestState<decimal, char, byte>>(id);
				registry.Add<TestState<char, bool, int>>(id);
				registry.Add<TestState<bool, byte, sbyte>>(id);
				registry.Add<TestState<sbyte, float, ushort>>(id);
				registry.Add<TestState<int, double, bool>>(id);
				registry.Add<TestState<double, decimal, char>>(id);
				registry.Add<TestState<long, bool, float>>(id);
				registry.Add<TestState<short, int, ulong>>(id);
				registry.Add<TestState<ushort, sbyte, decimal>>(id);
				registry.Add<TestState<ulong, char, double>>(id);
				registry.Add<TestState<decimal, bool, short>>(id);
				registry.Add<TestState<char, int, byte>>(id);
				registry.Add<TestState<bool, ulong, sbyte>>(id);
				registry.Add<TestState<sbyte, short, ushort>>(id);
				registry.Add<TestState<float, ulong, int>>(id);
				registry.Add<TestState<double, sbyte, long>>(id);
				registry.Add<TestState<int, char, decimal>>(id);
				registry.Add<TestState<long, decimal, bool>>(id);
				registry.Add<TestState<short, double, byte>>(id);
				registry.Add<TestState<ushort, float, char>>(id);
				registry.Add<TestState<ulong, int, bool>>(id);
				registry.Add<TestState<decimal, short, ulong>>(id);
				registry.Add<TestState<char, ushort, float>>(id);
				registry.Add<TestState<bool, double, sbyte>>(id);
				registry.Add<TestState<sbyte, long, ushort>>(id);
				registry.Add<TestState<float, decimal, char>>(id);
				registry.Add<TestState<double, byte, int>>(id);
				registry.Add<TestState<int, sbyte, ulong>>(id);
				registry.Add<TestState<long, ushort, decimal>>(id);
				registry.Add<TestState<short, char, double>>(id);
				registry.Add<TestState<ushort, bool, float>>(id);
				registry.Add<TestState<ulong, byte, short>>(id);
				registry.Add<TestState<decimal, int, char>>(id);
				registry.Add<TestState<char, ulong, bool>>(id);
				registry.Add<TestState<bool, short, double>>(id);
				registry.Add<TestState<sbyte, decimal, ushort>>(id);
				registry.Add<TestState<float, bool, byte>>(id);
				registry.Add<TestState<double, ushort, sbyte>>(id);
				registry.Add<TestState<int, float, char>>(id);
				registry.Add<TestState<long, byte, ulong>>(id);
				registry.Add<TestState<short, decimal, bool>>(id);
				registry.Add<TestState<ushort, double, int>>(id);
			}

			return registry;
		}
	}
}
using Massive.ECS;

namespace Massive.Samples.Benchmark
{
	public static class BenchmarkUtils
	{
		public static Registry GetSimplyPackedRegistry(int framesCapacity, int entitiesCapacity)
		{
			Registry registry = new Registry(framesCapacity + 1, entitiesCapacity);

			for (int i = 0; i < entitiesCapacity; i++)
			{
				registry.CreateEntity().Add<TestState>();
			}

			return registry;
		}

		public static Registry GetFullyPackedRegistry(int framesCapacity, int entitiesCapacity)
		{
			Registry registry = new Registry(framesCapacity + 1, entitiesCapacity);

			for (int i = 0; i < entitiesCapacity; i++)
			{
				// 50 different components
				registry.CreateEntity()
					.Add<TestState>()
					.Add<TestState<float, byte, int>>()
					.Add<TestState<int, float, double>>()
					.Add<TestState<double, int, long>>()
					.Add<TestState<long, short, byte>>()
					.Add<TestState<short, ushort, int>>()
					.Add<TestState<ushort, ulong, float>>()
					.Add<TestState<ulong, double, decimal>>()
					.Add<TestState<decimal, char, byte>>()
					.Add<TestState<char, bool, int>>()
					.Add<TestState<bool, byte, sbyte>>()
					.Add<TestState<sbyte, float, ushort>>()
					.Add<TestState<int, double, bool>>()
					.Add<TestState<double, decimal, char>>()
					.Add<TestState<long, bool, float>>()
					.Add<TestState<short, int, ulong>>()
					.Add<TestState<ushort, sbyte, decimal>>()
					.Add<TestState<ulong, char, double>>()
					.Add<TestState<decimal, bool, short>>()
					.Add<TestState<char, int, byte>>()
					.Add<TestState<bool, ulong, sbyte>>()
					.Add<TestState<sbyte, short, ushort>>()
					.Add<TestState<float, ulong, int>>()
					.Add<TestState<double, sbyte, long>>()
					.Add<TestState<int, char, decimal>>()
					.Add<TestState<long, decimal, bool>>()
					.Add<TestState<short, double, byte>>()
					.Add<TestState<ushort, float, char>>()
					.Add<TestState<ulong, int, bool>>()
					.Add<TestState<decimal, short, ulong>>()
					.Add<TestState<char, ushort, float>>()
					.Add<TestState<bool, double, sbyte>>()
					.Add<TestState<sbyte, long, ushort>>()
					.Add<TestState<float, decimal, char>>()
					.Add<TestState<double, byte, int>>()
					.Add<TestState<int, sbyte, ulong>>()
					.Add<TestState<long, ushort, decimal>>()
					.Add<TestState<short, char, double>>()
					.Add<TestState<ushort, bool, float>>()
					.Add<TestState<ulong, byte, short>>()
					.Add<TestState<decimal, int, char>>()
					.Add<TestState<char, ulong, bool>>()
					.Add<TestState<bool, short, double>>()
					.Add<TestState<sbyte, decimal, ushort>>()
					.Add<TestState<float, bool, byte>>()
					.Add<TestState<double, ushort, sbyte>>()
					.Add<TestState<int, float, char>>()
					.Add<TestState<long, byte, ulong>>()
					.Add<TestState<short, decimal, bool>>()
					.Add<TestState<ushort, double, int>>();
			}

			return registry;
		}
	}
}
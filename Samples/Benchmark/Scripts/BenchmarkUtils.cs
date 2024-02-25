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
				int entity = registry.CreateEntity();
				registry.Add<TestState>(entity);
			}

			return registry;
		}
		
		public static Registry GetFullyPackedRegistry(int framesCapacity, int entitiesCapacity)
		{
			Registry registry = new Registry(framesCapacity + 1, entitiesCapacity);
			
			for (int i = 0; i < entitiesCapacity; i++)
			{
				int entity = registry.CreateEntity();
				
				// 50 different components
				registry.Add<TestState>(entity);
				registry.Add<TestState<float, byte, int>>(entity);
				registry.Add<TestState<int, float, double>>(entity);
				registry.Add<TestState<double, int, long>>(entity);
				registry.Add<TestState<long, short, byte>>(entity);
				registry.Add<TestState<short, ushort, int>>(entity);
				registry.Add<TestState<ushort, ulong, float>>(entity);
				registry.Add<TestState<ulong, double, decimal>>(entity);
				registry.Add<TestState<decimal, char, byte>>(entity);
				registry.Add<TestState<char, bool, int>>(entity);
				registry.Add<TestState<bool, byte, sbyte>>(entity);
				registry.Add<TestState<sbyte, float, ushort>>(entity);
				registry.Add<TestState<int, double, bool>>(entity);
				registry.Add<TestState<double, decimal, char>>(entity);
				registry.Add<TestState<long, bool, float>>(entity);
				registry.Add<TestState<short, int, ulong>>(entity);
				registry.Add<TestState<ushort, sbyte, decimal>>(entity);
				registry.Add<TestState<ulong, char, double>>(entity);
				registry.Add<TestState<decimal, bool, short>>(entity);
				registry.Add<TestState<char, int, byte>>(entity);
				registry.Add<TestState<bool, ulong, sbyte>>(entity);
				registry.Add<TestState<sbyte, short, ushort>>(entity);
				registry.Add<TestState<float, ulong, int>>(entity);
				registry.Add<TestState<double, sbyte, long>>(entity);
				registry.Add<TestState<int, char, decimal>>(entity);
				registry.Add<TestState<long, decimal, bool>>(entity);
				registry.Add<TestState<short, double, byte>>(entity);
				registry.Add<TestState<ushort, float, char>>(entity);
				registry.Add<TestState<ulong, int, bool>>(entity);
				registry.Add<TestState<decimal, short, ulong>>(entity);
				registry.Add<TestState<char, ushort, float>>(entity);
				registry.Add<TestState<bool, double, sbyte>>(entity);
				registry.Add<TestState<sbyte, long, ushort>>(entity);
				registry.Add<TestState<float, decimal, char>>(entity);
				registry.Add<TestState<double, byte, int>>(entity);
				registry.Add<TestState<int, sbyte, ulong>>(entity);
				registry.Add<TestState<long, ushort, decimal>>(entity);
				registry.Add<TestState<short, char, double>>(entity);
				registry.Add<TestState<ushort, bool, float>>(entity);
				registry.Add<TestState<ulong, byte, short>>(entity);
				registry.Add<TestState<decimal, int, char>>(entity);
				registry.Add<TestState<char, ulong, bool>>(entity);
				registry.Add<TestState<bool, short, double>>(entity);
				registry.Add<TestState<sbyte, decimal, ushort>>(entity);
				registry.Add<TestState<float, bool, byte>>(entity);
				registry.Add<TestState<double, ushort, sbyte>>(entity);
				registry.Add<TestState<int, float, char>>(entity);
				registry.Add<TestState<long, byte, ulong>>(entity);
				registry.Add<TestState<short, decimal, bool>>(entity);
				registry.Add<TestState<ushort, double, int>>(entity);
			}

			return registry;
		}
	}
}
using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	public class MassiveRegistryPerformanceTest
	{
		private const int EntitiesCount = 10000;
		private const int MeasurementCount = 100;

		private readonly MassiveRegistry _registry;

		public MassiveRegistryPerformanceTest()
		{
			_registry = PrepareTestRegistry();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MassiveRegistry PrepareTestRegistry()
		{
			var config = new MassiveRegistryConfig(framesCapacity: 2);
			var registry = new MassiveRegistry(config);

			for (int i = 0; i < EntitiesCount; i++)
			{
				var entity = registry.Create();
				registry.Assign<TestComponent>(entity);
				registry.Assign<TestComponent1>(entity);
				registry.Assign<TestComponent2>(entity);
				registry.Assign<TestComponent3>(entity);
				registry.Assign<TestComponent4>(entity);
				registry.Assign<TestComponent5>(entity);
			}
			
			registry.SaveFrame();
			registry.SaveFrame();

			return registry;
		}

		[Test, Performance]
		public void MassiveRegistry_PopulateEntities()
		{
			Measure.Method(() => { PrepareTestRegistry(); })
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(1)
				.CleanUp(GC.Collect)
				.Run();
		}

		[Test, Performance]
		public void MassiveRegistry_Save()
		{
			Measure.Method(() =>
				{
					_registry.SaveFrame();
				})
				.MeasurementCount(MeasurementCount)
				.Run();
		}

		public struct TestComponent { public int Data; }
		public struct TestComponent1 { }
		public struct TestComponent2 { public int Data; }
		public struct TestComponent3 { public int Data; }
		public struct TestComponent4 { public int Data; }
		public struct TestComponent5 { public int Data; }
	}
}

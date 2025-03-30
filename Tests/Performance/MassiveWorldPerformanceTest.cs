using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	public class MassiveWorldPerformanceTest
	{
		private const int EntitiesCount = 10000;
		private const int MeasurementCount = 100;

		private readonly MassiveWorld _world;

		public MassiveWorldPerformanceTest()
		{
			_world = PrepareTestWorld();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static MassiveWorld PrepareTestWorld()
		{
			var config = new MassiveWorldConfig(framesCapacity: 2);
			var world = new MassiveWorld(config);

			for (int i = 0; i < EntitiesCount; i++)
			{
				var entity = world.Create();
				world.Add<TestComponent>(entity);
				world.Add<TestComponent1>(entity);
				world.Add<TestComponent2>(entity);
				world.Add<TestComponent3>(entity);
				world.Add<TestComponent4>(entity);
				world.Add<TestComponent5>(entity);
			}
			
			world.SaveFrame();
			world.SaveFrame();

			return world;
		}

		[Test, Performance]
		public void MassiveWorld_PopulateEntities()
		{
			Measure.Method(() => { PrepareTestWorld(); })
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(1)
				.CleanUp(GC.Collect)
				.Run();
		}

		[Test, Performance]
		public void MassiveWorld_Save()
		{
			Measure.Method(() =>
				{
					_world.SaveFrame();
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

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	public class FilterView3Include3IgnorePerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly World _world;
		private readonly Filter _filter;

		public FilterView3Include3IgnorePerformanceTest()
		{
			_world = new World().FillWorldWith50Components(EntitiesCount);

			_filter = GetTestFilter(_world);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Filter GetTestFilter(World world)
		{
			return new DynamicFilter(world)
				.AddToAll<TestState64>()
				.AddToAll<TestState64_2>()
				.AddToAll<TestState64_3>()
				.AddToNone<TestState64<byte, int, int>>()
				.AddToNone<TestState64<int, byte, int>>()
				.AddToNone<TestState64<int, int, byte>>();
		}

		[Test, Performance]
		public void FilterView_Fill()
		{
			var result = new List<int>();
			Measure.Method(() => _world.Filter(_filter).Fill(result))
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterView_FillEntities()
		{
			var result = new List<Entifier>();
			Measure.Method(() => _world.Filter(_filter).Fill(result))
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterView_Enumerator()
		{
			Measure.Method(() =>
				{
					foreach (var entityId in _world.Filter(_filter))
					{
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterView_ForEach()
		{
			Measure.Method(() => _world.Filter(_filter).ForEach((int _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewT_ForEach()
		{
			Measure.Method(() => _world.Filter(_filter).ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTT_ForEach()
		{
			Measure.Method(() => _world.Filter(_filter).ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach()
		{
			Measure.Method(() => _world.Filter(_filter).ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach_NoFilterCache()
		{
			Measure.Method(() => _world.Filter(GetTestFilter(_world))
					.ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

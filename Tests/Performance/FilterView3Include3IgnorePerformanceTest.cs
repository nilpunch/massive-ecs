using System;
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

		private readonly Registry _registry;
		private readonly Filter _filter;

		public FilterView3Include3IgnorePerformanceTest()
		{
			_registry = new Registry().FillRegistryWith50Components(EntitiesCount);

			_filter = GetTestFilter(_registry);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Filter GetTestFilter(Registry registry)
		{
			return registry.Filter<
				Include<TestState64, TestState64_2, TestState64_3>,
				Exclude<TestState64<byte, int, int>, TestState64<int, byte, int>, TestState64<int, int, byte>>>();
		}

		[Test, Performance]
		public void FilterView_Fill()
		{
			var result = new List<int>();
			Measure.Method(() => _registry.View().Filter(_filter).Fill(result))
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterView_FillEntities()
		{
			var result = new List<Entity>();
			Measure.Method(() => _registry.View().Filter(_filter).Fill(result))
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
					foreach (var entityId in _registry.View().Filter(_filter))
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
			Measure.Method(() => _registry.View().Filter(_filter).ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewT_ForEach()
		{
			Measure.Method(() => _registry.View().Filter(_filter).ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTT_ForEach()
		{
			Measure.Method(() => _registry.View().Filter(_filter).ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach()
		{
			Measure.Method(() => _registry.View().Filter(_filter).ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach_NoFilterCache()
		{
			Measure.Method(() => _registry.View().Filter(GetTestFilter(_registry))
					.ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

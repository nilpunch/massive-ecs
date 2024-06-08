using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	public class FilterView3Include3IgnorePerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly IRegistry _registry;
		private readonly IFilter _filter;

		public FilterView3Include3IgnorePerformanceTest()
		{
			_registry = new Registry().FillRegistryWith50Components(EntitiesCount);

			_filter = _registry.Filter<
				Include<TestState64, TestState64_2, TestState64_3>,
				Exclude<TestState64<byte, int, int>, TestState64<int, byte, int>, TestState64<int, int, byte>>>();
		}

		[Test, Performance]
		public void FilterView_ForEach()
		{
			Measure.Method(() => _registry.FilterView(_filter).ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewT_ForEach()
		{
			Measure.Method(() => _registry.FilterView<TestState64>(_filter).ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTT_ForEach()
		{
			Measure.Method(() => _registry.FilterView<TestState64, TestState64_2>(_filter).ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach()
		{
			Measure.Method(() => _registry.FilterView<TestState64, TestState64_2, TestState64_3>(_filter).ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

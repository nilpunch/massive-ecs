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

			var includeFilter = new IReadOnlySet[]
			{
				_registry.Any<TestState64>(),
				_registry.Any<TestState64_2>(),
				_registry.Any<TestState64_3>(),
			};

			var excludeFilter = new IReadOnlySet[]
			{
				_registry.Any<TestState64<byte, int, int>>(),
				_registry.Any<TestState64<int, byte, int>>(),
				_registry.Any<TestState64<int, int, byte>>(),
			};
			
			_filter = new Filter(includeFilter, excludeFilter);
		}

		[Test, Performance]
		public void FilterView_ForEach()
		{
			FilterView view = new FilterView(_registry, _filter);

			Measure.Method(() => view.ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewT_ForEach()
		{
			FilterView<TestState64> view = new FilterView<TestState64>(_registry, _filter);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTT_ForEach()
		{
			FilterView<TestState64, TestState64_2> view = new FilterView<TestState64, TestState64_2>(_registry, _filter);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach()
		{
			FilterView<TestState64, TestState64_2, TestState64_3> view = new FilterView<TestState64, TestState64_2, TestState64_3>(_registry, _filter);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}
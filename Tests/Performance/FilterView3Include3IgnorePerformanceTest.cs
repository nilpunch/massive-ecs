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
		private readonly ISet[] _includeFilter;
		private readonly ISet[] _excludeFilter;

		public FilterView3Include3IgnorePerformanceTest()
		{
			_registry = new Registry(EntitiesCount).FillRegistryWith50Components();

			_includeFilter = new ISet[]
			{
				_registry.Any<TestState64>(),
				_registry.Any<TestState64_2>(),
				_registry.Any<TestState64_3>(),
			};

			_excludeFilter = new ISet[]
			{
				_registry.Any<TestState64<byte, int, int>>(),
				_registry.Any<TestState64<int, byte, int>>(),
				_registry.Any<TestState64<int, int, byte>>(),
			};
		}

		[Test, Performance]
		public void FilterView_ForEach()
		{
			FilterView view = new FilterView(_registry, _includeFilter, _excludeFilter);

			Measure.Method(() => view.ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewT_ForEach()
		{
			FilterView<TestState64> view = new FilterView<TestState64>(_registry, _includeFilter, _excludeFilter);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTT_ForEach()
		{
			FilterView<TestState64, TestState64_2> view = new FilterView<TestState64, TestState64_2>(_registry, _includeFilter, _excludeFilter);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach()
		{
			FilterView<TestState64, TestState64_2, TestState64_3> view = new FilterView<TestState64, TestState64_2, TestState64_3>(_registry, _includeFilter, _excludeFilter);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}
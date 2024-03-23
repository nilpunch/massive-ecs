using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture(1000)]
	public class FilterViewPerformanceTest
	{
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly IRegistry _registry;

		public FilterViewPerformanceTest(int entitiesCount)
		{
			_registry = new Registry(entitiesCount).FillRegistryWith50Components();
		}

		[Test, Performance]
		public void FilterView_Performance()
		{
			FilterView view = new FilterView(_registry);

			Measure.Method(() => view.ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewT_Performance()
		{
			FilterView<TestState64> view = new FilterView<TestState64>(_registry);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTT_Performance()
		{
			FilterView<TestState64, TestState64_2> view = new FilterView<TestState64, TestState64_2>(_registry);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_Performance()
		{
			FilterView<TestState64, TestState64_2, TestState64_3> view = new FilterView<TestState64, TestState64_2, TestState64_3>(_registry);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}
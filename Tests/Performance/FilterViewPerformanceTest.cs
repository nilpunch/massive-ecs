using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture]
	public class FilterViewPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly IRegistry _registry;

		public FilterViewPerformanceTest()
		{
			_registry = new Registry().FillRegistryWith50Components(EntitiesCount);
		}

		[Test, Performance]
		public void FilterView_ForEach()
		{
			Measure.Method(() => _registry.FilterView().ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewT_ForEach()
		{
			Measure.Method(() => _registry.FilterView<TestState64>().ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTT_ForEach()
		{
			Measure.Method(() => _registry.FilterView<TestState64, TestState64_2>().ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach()
		{
			Measure.Method(() => _registry.FilterView<TestState64, TestState64_2, TestState64_3>().ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

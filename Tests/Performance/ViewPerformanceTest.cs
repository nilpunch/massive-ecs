using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture]
	public class ViewPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly IRegistry _registry;

		public ViewPerformanceTest()
		{
			_registry = new Registry().FillRegistryWith50Components(EntitiesCount);
		}

		[Test, Performance]
		public void View_ForEach()
		{
			Measure.Method(() => _registry.View().ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void ViewT_ForEach()
		{
			Measure.Method(() => _registry.View<TestState64>().ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void ViewTT_ForEach()
		{
			Measure.Method(() => _registry.View<TestState64, TestState64_2>().ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void ViewTTT_ForEach()
		{
			Measure.Method(() => _registry.View<TestState64, TestState64_2, TestState64_3>().ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void ViewT_ForEachExtra()
		{
			Measure.Method(() => _registry.View<TestState64>().ForEachExtra(0.016f, (int _, ref TestState64 _, float dt) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void ViewTT_ForEachExtra()
		{
			Measure.Method(() => _registry.View<TestState64, TestState64_2>().ForEachExtra(0.016f, (int _, ref TestState64 _, ref TestState64_2 _, float dt) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void ViewTTT_ForEachExtra()
		{
			Measure.Method(() => _registry.View<TestState64, TestState64_2, TestState64_3>().ForEachExtra(0.016f, (int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _, float dt) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

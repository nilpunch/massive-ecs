using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture]
	public class GroupViewPerformanceTest
	{
		private const int EntitiesCount = 100;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly IRegistry _registry;
		private readonly IGroup _group;

		public GroupViewPerformanceTest()
		{
			_registry = new Registry().FillRegistryWith50Components(EntitiesCount);
			_group = _registry.Group(_registry.Many<TestState64, TestState64_2, TestState64_3>());
		}

		[Test, Performance]
		public void GroupViewT_ForEach()
		{
			var view = new GroupView<TestState64>(_registry, _group);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _) =>
				{
				}))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTT_ForEach()
		{
			var view = new GroupView<TestState64, TestState64_2>(_registry, _group);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _) =>
				{
				}))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTTT_ForEach()
		{
			var view = new GroupView<TestState64, TestState64_2, TestState64_3>(_registry, _group);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) =>
				{
				}))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

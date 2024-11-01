using System.Collections.Generic;
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

		private readonly Registry _registry;

		public FilterViewPerformanceTest()
		{
			_registry = new Registry().FillRegistryWith50Components(EntitiesCount);
		}

		[Test, Performance]
		public void FilterView_Fill()
		{
			var result = new List<int>();
			Measure.Method(() => _registry.View().Filter().Fill(result))
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterView_FillEntities()
		{
			var result = new List<Entity>();
			Measure.Method(() => _registry.View().Filter().Fill(result))
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterView_ForEach()
		{
			Measure.Method(() =>
				{
					foreach (var entityId in _registry.View().Filter())
					{
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewT_ForEach()
		{
			Measure.Method(() =>
				{
					var dataSet = _registry.DataSet<TestState64>();
					foreach (var id in _registry.View().Include<TestState64>())
					{
						dataSet.Get(id);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTT_ForEach()
		{
			Measure.Method(() =>
				{
					var dataSet1 = _registry.DataSet<TestState64>();
					var dataSet2 = _registry.DataSet<TestState64_2>();
					foreach (var id in _registry.View().Include<TestState64, TestState64_2>())
					{
						dataSet1.Get(id);
						dataSet2.Get(id);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void FilterViewTTT_ForEach()
		{
			Measure.Method(() =>
				{
					var dataSet1 = _registry.DataSet<TestState64>();
					var dataSet2 = _registry.DataSet<TestState64_2>();
					var dataSet3 = _registry.DataSet<TestState64_3>();
					foreach (var id in _registry.View().Include<TestState64, TestState64_2, TestState64_3>())
					{
						dataSet1.Get(id);
						dataSet2.Get(id);
						dataSet3.Get(id);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

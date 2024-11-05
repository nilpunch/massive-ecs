using System;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	public class GroupPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		[Test, Performance]
		public void Group_Owning3()
		{
			var registry = new Registry().FillRegistryWith50Components(EntitiesCount);
			var group = registry.Group<None, None, Own<TestState64, TestState64_2, TestState64_3>>();

			Measure.Method(() =>
				{
					var pool1 = registry.DataSet<TestState64>();
					var pool2 = registry.DataSet<TestState64_2>();
					var pool3 = registry.DataSet<TestState64_3>();
					var groupSet = group.MainSet;

					foreach (var page in new GroupPageSequence(registry.PageSize, group))
					{
						var page1 = pool1.Data.Pages[page.Index];
						var page2 = pool2.Data.Pages[page.Index];
						var page3 = pool3.Data.Pages[page.Index];

						foreach (var entry in page)
						{
							// var id = groupSet.Ids[indexInPage + page.Offset];
							ref var data1 = ref page1[entry.Index];
							ref var data2 = ref page2[entry.Index];
							ref var data3 = ref page3[entry.Index];
						}
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Group_Include3()
		{
			var registry = new Registry().FillRegistryWith50Components(EntitiesCount);
			var group = registry.Group<Include<TestState64, TestState64_2, TestState64_3>>();

			Measure.Method(() =>
				{
					var pool1 = registry.DataSet<TestState64>();
					var pool2 = registry.DataSet<TestState64_2>();
					var pool3 = registry.DataSet<TestState64_3>();

					foreach (var id in registry.View().Group(group))
					{
						pool1.Get(id);
						pool2.Get(id);
						pool3.Get(id);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

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
		public void Group_Include3()
		{
			var world = new World().FillWorldWith50Components(EntitiesCount);
			var group = world.Group<Include<TestState64, TestState64_2, TestState64_3>>();

			Measure.Method(() =>
				{
					var pool1 = world.DataSet<TestState64>();
					var pool2 = world.DataSet<TestState64_2>();
					var pool3 = world.DataSet<TestState64_3>();

					foreach (var id in group)
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

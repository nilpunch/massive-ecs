using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixtureSource(nameof(FixtureSets))]
	public class SparseSetPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly ISet _set;

		public SparseSetPerformanceTest(ISet set)
		{
			_set = set;
		}

		public static ISet[] FixtureSets =
		{
			new SparseSet(EntitiesCount),
			new DataSet<TestState64>(EntitiesCount),
		};

		[Test, Performance]
		public void SparseSet_Ensure()
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_set.Ensure(i);
					}
				})
				.CleanUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_set.Remove(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void SparseSet_Remove()
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_set.Remove(i);
					}
				})
				.SetUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_set.Ensure(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
		
		[Test, Performance]
		public void SparseSet_RemoveNonExisting()
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_set.Remove(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}
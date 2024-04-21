using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	public class SparseSetPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		public static ISet[] FixtureSets =
		{
			new SparseSet(EntitiesCount),
			new DataSet<TestState64>(EntitiesCount),
		};

		public static ISet[] FixtureMassiveSets =
		{
			new MassiveSparseSet(EntitiesCount),
			new MassiveDataSet<TestState64>(EntitiesCount),
		};

		[TestCaseSource(nameof(FixtureSets)), Performance]
		public void SparseSet_Ensure(ISet set)
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Assign(i);
					}
				})
				.CleanUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Unassign(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[TestCaseSource(nameof(FixtureSets)), Performance]
		public void SparseSet_Remove(ISet set)
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Unassign(i);
					}
				})
				.SetUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Assign(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
		
		[TestCaseSource(nameof(FixtureSets)), Performance]
		public void SparseSet_RemoveNonExisting(ISet set)
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Unassign(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[TestCaseSource(nameof(FixtureMassiveSets)), Performance]
		public void MassiveSparseSet_Save(ISet set)
		{
			IMassive massive = (IMassive)set;

			for (int i = 0; i < EntitiesCount; i++)
			{
				set.Assign(i);
			}

			Measure.Method(() =>
				{
					massive.SaveFrame();
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}
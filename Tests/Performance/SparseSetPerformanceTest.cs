using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	public class SparseSetPerformanceTest
	{
		private const int EntitiesCount = 10000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		public static SparseSet[] FixtureSets =
		{
			new SparseSet(),
			new SparseSet(packing: Packing.WithHoles),
			new DataSet<TestState64>(),
			new DataSet<TestState64Stable>(packing: Packing.WithHoles),
		};

		public static DataSet<TestState64>[] FixtureDataSets =
		{
			new DataSet<TestState64>(),
			new DataSet<TestState64>(packing: Packing.WithHoles),
		};

		public static SparseSet[] FixtureMassiveSets =
		{
			new MassiveSparseSet(2),
			new MassiveSparseSet(2, packing: Packing.WithHoles),
			new MassiveDataSet<TestState64>(2),
			new MassiveDataSet<TestState64Stable>(2, packing: Packing.WithHoles),
		};

		[TestCaseSource(nameof(FixtureSets)), Performance]
		public void SparseSet_Ensure(SparseSet set)
		{
			set.Clear();
			set.ResizeSparse(0);
			set.ResizePacked(0);
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Add(i);
					}
				})
				.CleanUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Remove(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[TestCaseSource(nameof(FixtureSets)), Performance]
		public void SparseSet_Remove(SparseSet set)
		{
			set.Clear();
			set.ResizeSparse(0);
			set.ResizePacked(0);
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Remove(i);
					}
				})
				.SetUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Add(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
		
		[TestCaseSource(nameof(FixtureSets)), Performance]
		public void SparseSet_IsAssigned(SparseSet set)
		{
			set.Clear();
			set.ResizeSparse(0);
			set.ResizePacked(0);

			for (int i = 0; i < EntitiesCount; i++)
			{
				set.Add(i);
			}

			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Has(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[TestCaseSource(nameof(FixtureSets)), Performance]
		public void SparseSet_RemoveNonExisting(SparseSet set)
		{
			set.Clear();
			set.ResizeSparse(0);
			set.ResizePacked(0);
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Remove(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
		
		[TestCaseSource(nameof(FixtureDataSets)), Performance]
		public void MassiveSparseSet_Get(DataSet<TestState64> set)
		{
			set.Clear();
			set.ResizeSparse(0);
			set.ResizePacked(0);

			for (int i = 0; i < EntitiesCount; i++)
			{
				set.Add(i);
			}

			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						set.Get(i);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[TestCaseSource(nameof(FixtureMassiveSets)), Performance]
		public void MassiveSparseSet_Save(SparseSet set)
		{
			set.Clear();
			set.ResizeSparse(0);
			set.ResizePacked(0);
			IMassive massive = (IMassive)set;

			for (int i = 0; i < EntitiesCount; i++)
			{
				set.Add(i);
			}

			Measure.Method(() => { massive.SaveFrame(); })
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
		
		[TestCaseSource(nameof(FixtureMassiveSets)), Performance]
		public void MassiveSparseSet_BigSave(SparseSet set)
		{
			set.Clear();
			set.ResizeSparse(0);
			set.ResizePacked(0);
			IMassive massive = (IMassive)set;

			for (int i = 0; i < EntitiesCount * IterationsPerMeasurement; i++)
			{
				set.Add(i);
			}
			
			Measure.Method(() => { massive.SaveFrame(); })
				.WarmupCount(Constants.DefaultFramesCapacity)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(1)
				.Run();
		}
	}
}

﻿using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	public class SparseSetPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		public static SparseSet[] FixtureSets =
		{
			new SparseSet(EntitiesCount),
			new DataSet<TestState64>(EntitiesCount),
		};

		public static SparseSet[] FixtureMassiveSets =
		{
			new MassiveSparseSet(EntitiesCount),
			new MassiveDataSet<TestState64>(EntitiesCount),
		};

		[TestCaseSource(nameof(FixtureSets)), Performance]
		public void SparseSet_Ensure(SparseSet set)
		{
			set.Clear();
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
		public void SparseSet_Remove(SparseSet set)
		{
			set.Clear();
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
		public void SparseSet_RemoveNonExisting(SparseSet set)
		{
			set.Clear();
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
		public void MassiveSparseSet_Save(SparseSet set)
		{
			set.Clear();
			IMassive massive = (IMassive)set;

			for (int i = 0; i < EntitiesCount; i++)
			{
				set.Assign(i);
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
			IMassive massive = (IMassive)set;

			for (int i = 0; i < EntitiesCount * IterationsPerMeasurement; i++)
			{
				set.Assign(i);
			}
			
			Measure.Method(() => { massive.SaveFrame(); })
				.WarmupCount(Constants.DefaultFramesCapacity)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(1)
				.Run();
		}
	}
}

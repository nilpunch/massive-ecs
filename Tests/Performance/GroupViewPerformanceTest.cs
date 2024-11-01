using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture(GroupSetupType.Owning3)]
	[TestFixture(GroupSetupType.Include3)]
	public class GroupViewPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly GroupSetupType _groupSetupType;
		private readonly Registry _registry;
		private readonly Group _group;

		public enum GroupSetupType
		{
			Owning3,
			Owning2Include1,
			Owning1Include2,
			Include3,
		}

		public GroupViewPerformanceTest(GroupSetupType groupSetupType)
		{
			_groupSetupType = groupSetupType;
			_registry = new Registry().FillRegistryWith50Components(EntitiesCount);
			_group = GetTestGroup(_registry, groupSetupType);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static Group GetTestGroup(Registry registry, GroupSetupType type)
		{
			return type switch
			{
				GroupSetupType.Owning3 => registry.Group<None, None, Own<TestState64, TestState64_2, TestState64_3>>(),
				GroupSetupType.Include3 => registry.Group<Include<TestState64, TestState64_2, TestState64_3>>(),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		}

		[Test, Performance]
		public void GroupView_Fill()
		{
			var result = new List<int>();
			Measure.Method(() => _registry.View().Group(_group).Fill(result))
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupView_FillEntities()
		{
			var result = new List<Entity>();
			Measure.Method(() => _registry.View().Group(_group).Fill(result))
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupView_ForEach()
		{
			Measure.Method(() =>
				{
					foreach (var entityId in _registry.View().Group(_group))
					{
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewT_ForEach()
		{
			Measure.Method(() =>
				{
					var pool = _registry.DataSet<TestState64>();

					var groupSet = _group.MainSet;

					if (_groupSetupType == GroupSetupType.Owning3)
					{
						foreach (var page in new GroupPageSequence(_registry.PageSize, _group))
						{
							var dataPage = pool.Data.Pages[page.Index];
					
							foreach (var indexInPage in page)
							{
								var id = groupSet.Ids[indexInPage + page.Offset];
								ref var data = ref dataPage[indexInPage];
							}
						}
					}
					else
					{
						foreach (var id in _registry.View().Group(_group))
						{
							pool.Get(id);
						}
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTT_ForEach()
		{
			Measure.Method(() =>
				{
					var pool1 = _registry.DataSet<TestState64>();
					var pool2 = _registry.DataSet<TestState64_2>();

					var groupSet = _group.MainSet;

					if (_groupSetupType == GroupSetupType.Owning3)
					{
						foreach (var page in new GroupPageSequence(_registry.PageSize, _group))
						{
							var page1 = pool1.Data.Pages[page.Index];
							var page2 = pool2.Data.Pages[page.Index];

							foreach (var indexInPage in page)
							{
								var id = groupSet.Ids[indexInPage + page.Offset];
								ref var data1 = ref page1[indexInPage];
								ref var data2 = ref page2[indexInPage];
							}
						}
					}
					else
					{
						foreach (var id in _registry.View().Group(_group))
						{
							pool1.Get(id);
							pool2.Get(id);
						}
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTTT_ForEach()
		{
			Measure.Method(() =>
				{
					var pool1 = _registry.DataSet<TestState64>();
					var pool2 = _registry.DataSet<TestState64_2>();
					var pool3 = _registry.DataSet<TestState64_3>();

					var groupSet = _group.MainSet;

					if (_groupSetupType == GroupSetupType.Owning3)
					{
						foreach (var page in new GroupPageSequence(_registry.PageSize, _group))
						{
							var page1 = pool1.Data.Pages[page.Index];
							var page2 = pool2.Data.Pages[page.Index];
							var page3 = pool3.Data.Pages[page.Index];

							foreach (var indexInPage in page)
							{
								var id = groupSet.Ids[indexInPage + page.Offset];
								ref var data1 = ref page1[indexInPage];
								ref var data2 = ref page2[indexInPage];
								ref var data3 = ref page3[indexInPage];
							}
						}
					}
					else
					{
						foreach (var id in _registry.View().Group(_group))
						{
							pool1.Get(id);
							pool2.Get(id);
							pool3.Get(id);
						}
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

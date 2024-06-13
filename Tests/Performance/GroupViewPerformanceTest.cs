using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture(GroupSetupType.Owning3)]
	[TestFixture(GroupSetupType.Owning2Include1)]
	[TestFixture(GroupSetupType.Owning1Include2)]
	[TestFixture(GroupSetupType.Include3)]
	public class GroupViewPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly GroupSetupType _groupSetupType;
		private readonly IRegistry _registry;
		private readonly IGroup _group;

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
		private static IGroup GetTestGroup(IRegistry registry, GroupSetupType type)
		{
			return type switch
			{
				GroupSetupType.Owning3 => registry.Group<Own<TestState64, TestState64_2, TestState64_3>>(),
				GroupSetupType.Owning2Include1 => registry.Group<Own<TestState64, TestState64_2>, Include<TestState64_3>>(),
				GroupSetupType.Owning1Include2 => registry.Group<Own<TestState64>, Include<TestState64_2, TestState64_3>>(),
				GroupSetupType.Include3 => registry.Group<None, Include<TestState64, TestState64_2, TestState64_3>>(),
				_ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
			};
		}

		[Test, Performance]
		public void GroupView_ForEach()
		{
			Measure.Method(() => _registry.GroupView(_group).ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupView_Fill()
		{
			var result = new List<int>();
			Measure.Method(() => _registry.GroupView(_group).Fill(result))
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewT_ForEach()
		{
			Measure.Method(() => _registry.GroupView<TestState64>(_group).ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTT_ForEach()
		{
			Measure.Method(() => _registry.GroupView<TestState64, TestState64_2>(_group).ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTTT_ForEach()
		{
			Measure.Method(() => _registry.GroupView<TestState64, TestState64_2, TestState64_3>(_group).ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTTT_ForEach_NoGroupCache()
		{
			Measure.Method(() => _registry.GroupView<TestState64, TestState64_2, TestState64_3>(GetTestGroup(_registry, _groupSetupType)).ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

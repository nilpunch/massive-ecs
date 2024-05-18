using System;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixtureSource(nameof(GroupSetupTypes))]
	public class GroupViewPerformanceTest
	{
		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly IRegistry _registry;
		private readonly IGroup _group;

		public enum GroupSetupType
		{
			Owning3,
			Owning2Include1,
			Owning1Include2,
			Include3,
		}

		public static GroupSetupType[] GroupSetupTypes = new[]
		{
			GroupSetupType.Owning3,
			GroupSetupType.Owning2Include1,
			GroupSetupType.Owning1Include2,
			GroupSetupType.Include3,
		};

		public static readonly Func<IRegistry, IGroup>[] GroupSetups = new Func<IRegistry, IGroup>[]
		{
			(registry) => registry.Group(registry.Many<TestState64, TestState64_2, TestState64_3>()),
			(registry) => registry.Group(registry.Many<TestState64, TestState64_2>(), registry.Many<TestState64_3>()),
			(registry) => registry.Group(registry.Many<TestState64>(), registry.Many<TestState64_2, TestState64_3>()),
			(registry) => registry.Group(null, registry.Many<TestState64, TestState64_2, TestState64_3>()),
		};

		public GroupViewPerformanceTest(GroupSetupType groupSetupType)
		{
			_registry = new Registry().FillRegistryWith50Components(EntitiesCount);
			_group = GroupSetups[(int)groupSetupType].Invoke(_registry);
		}

		[Test, Performance]
		public void GroupView_ForEach()
		{
			var view = new GroupView(_group);
		
			Measure.Method(() => view.ForEach((_) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewT_ForEach()
		{
			var view = new GroupView<TestState64>(_registry, _group);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTT_ForEach()
		{
			var view = new GroupView<TestState64, TestState64_2>(_registry, _group);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void GroupViewTTT_ForEach()
		{
			var view = new GroupView<TestState64, TestState64_2, TestState64_3>(_registry, _group);

			Measure.Method(() => view.ForEach((int _, ref TestState64 _, ref TestState64_2 _, ref TestState64_3 _) => { }))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

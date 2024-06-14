using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture(RegistryFilling.FillWithSingleComponent)]
	[TestFixture(RegistryFilling.FillWith50Components)]
	[TestFixture(RegistryFilling.FillWith50Tags)]
	public class RegistryPerformanceTest
	{
		private readonly RegistryFilling _registryFilling;

		public enum RegistryFilling
		{
			FillWithSingleComponent,
			FillWith50Components,
			FillWith50Tags,
		}

		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly IRegistry _registry;

		public RegistryPerformanceTest(RegistryFilling registryFilling)
		{
			_registryFilling = registryFilling;
			_registry = PrepareTestRegistry(registryFilling);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static IRegistry PrepareTestRegistry(RegistryFilling registryFilling)
		{
			return registryFilling switch
			{
				RegistryFilling.FillWithSingleComponent => new Registry().FillRegistryWithSingleComponent(1),
				RegistryFilling.FillWith50Components => new Registry().FillRegistryWith50Components(1),
				RegistryFilling.FillWith50Tags => new Registry().FillRegistryWith50Tags(1),
				_ => throw new ArgumentOutOfRangeException(nameof(_registryFilling))
			};
		}

		[Test, Performance]
		public void Registry_Initialization()
		{
			Measure.Method(() =>
				{
					PrepareTestRegistry(_registryFilling);
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(1)
				.CleanUp(GC.Collect)
				.Run();
		}

		[Test, Performance]
		public void Registry_Create()
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_registry.Create();
					}
				})
				.SetUp(() => _registry.View().ForEach((entity) => _registry.Destroy(entity)))
				.CleanUp(() => _registry.View().ForEach((entity) => _registry.Destroy(entity)))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
		
		[Test, Performance]
		public void Registry_Fill()
		{
			var result = new List<Entity>();

			_registry.View().ForEach((entity) => _registry.Destroy(entity));

			for (int i = 0; i < EntitiesCount; i++)
			{
				_registry.Create<TestState64>();
			}

			Measure.Method(() =>
				{
					_registry.Fill<Include<TestState64>>(result);
				})
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.View().ForEach((entity) => _registry.Destroy(entity));
		}

		[Test, Performance]
		public void Registry_Destroy()
		{
			Measure.Method(() => _registry.View().ForEachExtra(_registry, (entity, registry) => registry.Destroy(entity)))
				.SetUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_registry.Create();
					}
				})
				.CleanUp(() => _registry.View().ForEachExtra(_registry, (entity, registry) => registry.Destroy(entity)))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}

using System;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture(RegistryFilling.FillWithSingleComponent)]
	[TestFixture(RegistryFilling.FillWith50Components)]
	[TestFixture(RegistryFilling.FillWith50Tags)]
	public class RegistryPerformanceTest
	{
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
			_registry = registryFilling switch
			{
				RegistryFilling.FillWithSingleComponent => new Registry(EntitiesCount).FillRegistryWithSingleComponent(),
				RegistryFilling.FillWith50Components => new Registry(EntitiesCount).FillRegistryWith50Components(),
				RegistryFilling.FillWith50Tags => new Registry(EntitiesCount).FillRegistryWith50Tags(),
				_ => throw new ArgumentOutOfRangeException(nameof(registryFilling))
			};
		}

		[Test, Performance]
		public void Registry_Create()
		{
			View entities = new View(_registry);

			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_registry.Create();
					}
				})
				.SetUp(() => entities.ForEachExtra(_registry, (entity, registry) => registry.Destroy(entity)))
				.CleanUp(() => entities.ForEachExtra(_registry, (entity, registry) => registry.Destroy(entity)))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_Destroy()
		{
			View entities = new View(_registry);

			Measure.Method(() => entities.ForEachExtra(_registry, (entity, registry) => registry.Destroy(entity)))
				.SetUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_registry.Create();
					}
				})
				.CleanUp(() => entities.ForEachExtra(_registry, (entity, registry) => registry.Destroy(entity)))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}
	}
}
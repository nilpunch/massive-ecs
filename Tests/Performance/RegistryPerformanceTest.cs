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
			_registry.View().ForEach((entityId) => _registry.Destroy(entityId));
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
					_registry.View().Filter<Include<TestState64>>().Fill(result);
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
				.CleanUp(() => _registry.View().ForEach((entityId) => _registry.Destroy(entityId)))
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_GetTwoComponents()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				var entity = _registry.Create();
				_registry.Assign(entity, new PositionComponent() { X = i, Y = i });
				_registry.Assign(entity, new VelocityComponent() { X = 1, Y = 1 });
			}

			Measure.Method(() =>
				{
					_registry.View().ForEach((entityId) =>
					{
						_registry.Get<PositionComponent>(entityId);
						_registry.Get<VelocityComponent>(entityId);
					});
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.View().ForEach((entityId) => _registry.Destroy(entityId));
		}

		[Test, Performance]
		public void Registry_RemoveAndAddComponent()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				_registry.Create(new PositionComponent() { X = i, Y = i });
			}

			Measure.Method(() =>
				{
					_registry.View().ForEach((entityId) =>
					{
						_registry.Unassign<PositionComponent>(entityId);
						_registry.Assign(entityId, new PositionComponent() { X = entityId, Y = entityId });
					});
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.View().ForEach((entityId) =>
			{
				_registry.Destroy(entityId);
			});
		}

		[Test, Performance]
		public void Registry_RemoveAndAddComponentFast()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				_registry.Create(new PositionComponent() { X = i, Y = i });
			}

			Measure.Method(() =>
				{
					var positions = _registry.DataSet<PositionComponent>();
					_registry.View().ForEach((entityId) =>
					{
						positions.Unassign(entityId);
						positions.Assign(entityId, new PositionComponent() { X = entityId, Y = entityId });
					});
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.View().ForEach((entityId) =>
			{
				_registry.Destroy(entityId);
			});
		}

		public struct PositionComponent
		{
			public float X;
			public float Y;
		}

		public struct VelocityComponent
		{
			public float X;
			public float Y;
		}
	}
}

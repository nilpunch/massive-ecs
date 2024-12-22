using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture(RegistryFilling.x50Components, RegistryStability.FullStability)]
	[TestFixture(RegistryFilling.x50Components, RegistryStability.DefaultStability)]
	[TestFixture(RegistryFilling.SingleComponent, RegistryStability.DefaultStability)]
	[TestFixture(RegistryFilling.x50Tags, RegistryStability.DefaultStability)]
	public class RegistryPerformanceTest
	{
		private readonly RegistryFilling _registryFilling;
		private readonly bool _fullStability;

		public enum RegistryFilling
		{
			SingleComponent,
			x50Components,
			x50ComponentsPlusNonOwningGroup,
			x50Tags,
		}

		public enum RegistryStability
		{
			DefaultStability,
			FullStability,
		}

		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly Registry _registry;

		public RegistryPerformanceTest(RegistryFilling registryFilling, RegistryStability registryStability)
		{
			_registryFilling = registryFilling;
			_fullStability = registryStability == RegistryStability.FullStability;
			_registry = PrepareTestRegistry(_registryFilling, _fullStability);
			_registry.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Registry PrepareTestRegistry(RegistryFilling registryFilling, bool fullStability)
		{
			var config = new RegistryConfig(fullStability: fullStability);
			return registryFilling switch
			{
				RegistryFilling.SingleComponent => new Registry(config).FillRegistryWithSingleComponent(),
				RegistryFilling.x50Components => new Registry(config).FillRegistryWith50Components(),
				RegistryFilling.x50ComponentsPlusNonOwningGroup => new Registry(config).FillRegistryWith50Components().FillRegistryWithNonOwningGroup<Include<PositionComponent>>(),
				RegistryFilling.x50Tags => new Registry(config).FillRegistryWith50Tags(),
				_ => throw new ArgumentOutOfRangeException(nameof(_registryFilling))
			};
		}

		[Test, Performance]
		public void Registry_Initialization()
		{
			Measure.Method(() => { PrepareTestRegistry(_registryFilling, _fullStability); })
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
				.SetUp(() => _registry.Clear())
				.CleanUp(() => _registry.Clear())
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_CloneWithTwoComponents()
		{
			Measure.Method(() =>
				{
					var entityToClone = _registry.Create();
					_registry.Assign(entityToClone, new PositionComponent() { X = 2, Y = 2 });
					_registry.Assign(entityToClone, new VelocityComponent() { X = 1, Y = 1 });

					for (int i = 0; i < EntitiesCount; i++)
					{
						_registry.Clone(entityToClone);
					}
				})
				.SetUp(() => _registry.Clear())
				.CleanUp(() => _registry.Clear())
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_Destroy()
		{
			Measure.Method(() =>
				{
					_registry.Clear();
				})
				.SetUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						int id = _registry.Create();
						_registry.Assign<PositionComponent>(id);
					}
				})
				.CleanUp(() => _registry.Clear())
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_Fill()
		{
			var result = new List<Entity>();

			_registry.Clear();

			for (int i = 0; i < EntitiesCount; i++)
			{
				_registry.Create<TestState64>();
			}

			Measure.Method(() => { _registry.View().Filter<Include<TestState64>>().Fill(result); })
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.Clear();
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
					foreach (var entityId in _registry.View())
					{
						_registry.Get<PositionComponent>(entityId);
						_registry.Get<VelocityComponent>(entityId);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.Clear();
		}

		[Test, Performance]
		public void Registry_GetTwoComponentsFast()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				var entityId = _registry.Create();
				_registry.Assign(entityId, new PositionComponent() { X = i, Y = i });
				_registry.Assign(entityId, new VelocityComponent() { X = 1, Y = 1 });
			}

			Measure.Method(() =>
				{
					var positions = _registry.DataSet<PositionComponent>();
					var velocities = _registry.DataSet<VelocityComponent>();
					foreach (var entityId in _registry.View())
					{
						positions.Get(entityId);
						velocities.Get(entityId);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.Clear();
		}

		[Test, Performance]
		public void Registry_GetTwoComponentsViaView()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				var entity = _registry.Create();
				_registry.Assign(entity, new PositionComponent() { X = i, Y = i });
				_registry.Assign(entity, new VelocityComponent() { X = 1, Y = 1 });
			}

			Measure.Method(() => { _registry.View().ForEach((ref PositionComponent position, ref VelocityComponent velocity) => { }); })
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.Clear();
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
					foreach (var entityId in _registry.View())
					{
						_registry.Unassign<PositionComponent>(entityId);
						_registry.Assign(entityId, new PositionComponent() { X = entityId, Y = entityId });
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.Clear();
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
					foreach (var entityId in _registry.View())
					{
						positions.Unassign(entityId);
						positions.Assign(entityId, new PositionComponent() { X = entityId, Y = entityId });
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_registry.Clear();
		}

		[Test, Performance]
		public void Registry_ViewFiltation_Overhead()
		{
			Measure.Method(() =>
				{
					foreach (var i in _registry.View().Include<PositionComponent, VelocityComponent>())
					{
						break;
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(2000)
				.Run();
		}

		[Test, Performance]
		public void Registry_SetLookupOverhead()
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_registry.DataSet<PositionComponent>();
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
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

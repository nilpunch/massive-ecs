using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture(WorldFilling.x50Components, WorldStability.FullStability)]
	[TestFixture(WorldFilling.x50Components, WorldStability.DefaultStability)]
	[TestFixture(WorldFilling.SingleComponent, WorldStability.DefaultStability)]
	[TestFixture(WorldFilling.x50Tags, WorldStability.DefaultStability)]
	public class WorldPerformanceTest
	{
		private readonly WorldFilling _worldFilling;
		private readonly bool _fullStability;

		public enum WorldFilling
		{
			SingleComponent,
			x50Components,
			x50Tags,
		}

		public enum WorldStability
		{
			DefaultStability,
			FullStability,
		}

		private const int EntitiesCount = 1000;
		private const int MeasurementCount = 100;
		private const int IterationsPerMeasurement = 120;

		private readonly World _world;

		public WorldPerformanceTest(WorldFilling worldFilling, WorldStability worldStability)
		{
			_worldFilling = worldFilling;
			_fullStability = worldStability == WorldStability.FullStability;
			_world = PrepareTestRegistry(_worldFilling, _fullStability);
			_world.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static World PrepareTestRegistry(WorldFilling worldFilling, bool fullStability)
		{
			var config = new WorldConfig(fullStability: fullStability);
			return worldFilling switch
			{
				WorldFilling.SingleComponent => new World(config).FillWorldWithSingleComponent(),
				WorldFilling.x50Components => new World(config).FillWorldWith50Components(),
				WorldFilling.x50Tags => new World(config).FillWorldWith50Tags(),
				_ => throw new ArgumentOutOfRangeException(nameof(_worldFilling))
			};
		}

		[Test, Performance]
		public void Registry_Initialization()
		{
			Measure.Method(() => { PrepareTestRegistry(_worldFilling, _fullStability); })
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
						_world.Create();
					}
				})
				.SetUp(() => _world.Clear())
				.CleanUp(() => _world.Clear())
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_CloneWithTwoComponents()
		{
			Measure.Method(() =>
				{
					var entityToClone = _world.Create();
					_world.Set(entityToClone, new PositionComponent() { X = 2, Y = 2 });
					_world.Set(entityToClone, new VelocityComponent() { X = 1, Y = 1 });

					for (int i = 0; i < EntitiesCount; i++)
					{
						_world.Clone(entityToClone);
					}
				})
				.SetUp(() => _world.Clear())
				.CleanUp(() => _world.Clear())
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_Destroy()
		{
			Measure.Method(() =>
				{
					_world.Clear();
				})
				.SetUp(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						int id = _world.Create();
						_world.Add<PositionComponent>(id);
					}
				})
				.CleanUp(() => _world.Clear())
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_Fill()
		{
			var result = new List<Entity>();

			_world.Clear();

			for (int i = 0; i < EntitiesCount; i++)
			{
				_world.Create<TestState64>();
			}

			Measure.Method(() => { _world.View().Filter<Include<TestState64>>().Fill(result); })
				.CleanUp(result.Clear)
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_world.Clear();
		}

		[Test, Performance]
		public void Registry_GetTwoComponents()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				var entity = _world.Create();
				_world.Set(entity, new PositionComponent() { X = i, Y = i });
				_world.Set(entity, new VelocityComponent() { X = 1, Y = 1 });
			}

			Measure.Method(() =>
				{
					foreach (var entityId in _world.View())
					{
						_world.Get<PositionComponent>(entityId);
						_world.Get<VelocityComponent>(entityId);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_world.Clear();
		}

		[Test, Performance]
		public void Registry_GetTwoComponentsFast()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				var entityId = _world.Create();
				_world.Set(entityId, new PositionComponent() { X = i, Y = i });
				_world.Set(entityId, new VelocityComponent() { X = 1, Y = 1 });
			}

			Measure.Method(() =>
				{
					var positions = _world.DataSet<PositionComponent>();
					var velocities = _world.DataSet<VelocityComponent>();
					foreach (var entityId in _world.View())
					{
						positions.Get(entityId);
						velocities.Get(entityId);
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_world.Clear();
		}

		[Test, Performance]
		public void Registry_GetTwoComponentsViaView()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				var entity = _world.Create();
				_world.Set(entity, new PositionComponent() { X = i, Y = i });
				_world.Set(entity, new VelocityComponent() { X = 1, Y = 1 });
			}

			Measure.Method(() => { _world.View().ForEach((ref PositionComponent position, ref VelocityComponent velocity) => { }); })
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_world.Clear();
		}

		[Test, Performance]
		public void Registry_RemoveAndAddComponent()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				_world.Create(new PositionComponent() { X = i, Y = i });
			}

			Measure.Method(() =>
				{
					foreach (var entityId in _world.View())
					{
						_world.Remove<PositionComponent>(entityId);
						_world.Set(entityId, new PositionComponent() { X = entityId, Y = entityId });
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_world.Clear();
		}

		[Test, Performance]
		public void Registry_RemoveAndAddComponentFast()
		{
			for (int i = 0; i < EntitiesCount; i++)
			{
				_world.Create(new PositionComponent() { X = i, Y = i });
			}

			Measure.Method(() =>
				{
					var positions = _world.DataSet<PositionComponent>();
					foreach (var entityId in _world.View())
					{
						positions.Remove(entityId);
						positions.Set(entityId, new PositionComponent() { X = entityId, Y = entityId });
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();

			_world.Clear();
		}

		[Test, Performance]
		public void Registry_ViewFiltation_Overhead()
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_world.View().Include<PositionComponent, VelocityComponent>();
					}
				})
				.MeasurementCount(MeasurementCount)
				.IterationsPerMeasurement(IterationsPerMeasurement)
				.Run();
		}

		[Test, Performance]
		public void Registry_SetLookupOverhead()
		{
			Measure.Method(() =>
				{
					for (int i = 0; i < EntitiesCount; i++)
					{
						_world.DataSet<PositionComponent>();
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

using System.Numerics;
using Massive.Samples.Shooter;
using NUnit.Framework;
using Unity.PerformanceTesting;

namespace Massive.PerformanceTests
{
	[TestFixture]
	public class WorldShooterPerformanceTest
	{
		private const int CharacterRowsAmount = 10;

		[Test, Performance]
		public void SimulateShooter()
		{
			const int fps = 60;
			const int simulateSeconds = 30;

			Shooter shooter = new Shooter();

			Measure.Method(() => shooter.Systems.Invoke(shooter.World, 1f / fps))
				.SetUp(() =>
				{
					shooter.CreateCharactersInTwoOppositeRows(Vector2.Zero,
						new Vector2(0, 1f),
						new Vector2(1f, 0f),
						CharacterRowsAmount);
				})
				.CleanUp(() => shooter.World.View().ForEach((entityId) =>
				{
					shooter.World.Destroy(entityId);
				}))
				.WarmupCount(1)
				.MeasurementCount(20)
				.IterationsPerMeasurement(simulateSeconds * fps)
				.Run();
		}
	}
}

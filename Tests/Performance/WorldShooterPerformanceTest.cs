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

			Sample sample = new Sample();

			Measure.Method(() => sample.Systems.Run<IUpdate, float>(1f / fps))
				.SetUp(() =>
				{
					sample.CreateCharactersInTwoOppositeRows(Vector2.Zero,
						new Vector2(0, 1f),
						new Vector2(1f, 0f),
						CharacterRowsAmount);
				})
				.CleanUp(() => sample.World.ForEach((entityId) =>
				{
					sample.World.Destroy(entityId);
				}))
				.WarmupCount(1)
				.MeasurementCount(20)
				.IterationsPerMeasurement(simulateSeconds * fps)
				.Run();
		}
	}
}

using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class WorldStateTests
	{
		private struct TestState : IState
		{
			public int Value;

			public int SparseIndex { get; set; }
		}

		[Test]
		public void Delete_ShouldRewriteSparseIndices()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			worldState.Create(new TestState { Value = 1 });
			worldState.Create(new TestState { Value = 2 });
			worldState.Create(new TestState { Value = 3 });

			Assert.AreEqual(worldState.Get(0).SparseIndex, 0);
			Assert.AreEqual(worldState.Get(1).SparseIndex, 1);
			Assert.AreEqual(worldState.Get(2).SparseIndex, 2);

			worldState.Delete(1);

			Assert.IsFalse(worldState.IsAlive(1));
			Assert.AreEqual(worldState.Get(2).SparseIndex, 1);
		}

		[Test]
		public void Reserve_ShouldInitializeData()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			worldState.Create(new TestState { Value = 1 });
			worldState.Create(new TestState { Value = 2 });
			worldState.Create(new TestState { Value = 3 });

			Assert.AreEqual(worldState.Get(0).Value, 1);
			Assert.AreEqual(worldState.Get(1).Value, 2);
			Assert.AreEqual(worldState.Get(2).Value, 3);
		}

		[Test]
		public void Reserve_ShouldReturnCorrectSparseIndex()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			var index1 = worldState.Create(new TestState { Value = 1 });
			var index2 = worldState.Create(new TestState { Value = 2 });
			var index3 = worldState.Create(new TestState { Value = 3 });

			Assert.AreEqual(index1, 0);
			Assert.AreEqual(index2, 1);
			Assert.AreEqual(index3, 2);
		}

		[Test]
		public void State_WhenAffected_ShouldChangeState()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 2);

			var index1 = worldState.Create(new TestState { Value = 1 });

			worldState.Get(index1).Value = 2;

			Assert.AreEqual(worldState.Get(index1).Value, 2);
		}

		[Test]
		public void SaveFrame_ShouldPreserveStates()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			var index1 = worldState.Create(new TestState { Value = 1 });
			var index2 = worldState.Create(new TestState { Value = 2 });
			var index3 = worldState.Create(new TestState { Value = 3 });

			worldState.SaveFrame();

			Assert.AreEqual(worldState.Get(index1).Value, 1);
			Assert.AreEqual(worldState.Get(index2).Value, 2);
			Assert.AreEqual(worldState.Get(index3).Value, 3);
		}

		[Test]
		public void CurrentFrame_ShouldThrowWhenAccessedAfterFrameChange()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			Assert.DoesNotThrow(() =>
			{
				var currentFrame = worldState.CurrentFrame;
				currentFrame.IsAlive(1);
			});

			Assert.Catch(() =>
			{
				var currentFrame = worldState.CurrentFrame;
				worldState.SaveFrame();
				currentFrame.IsAlive(1);
			});
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 2);

			var index1 = worldState.Create(new TestState { Value = 1 });
			worldState.SaveFrame();

			worldState.Get(index1).Value = 2;
			worldState.Rollback(0);

			Assert.AreEqual(worldState.Get(index1).Value, 1);
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 2);

			worldState.SaveFrame();

			Assert.IsFalse(worldState.IsAlive(0));
			Assert.IsFalse(worldState.IsAlive(1));

			worldState.Create(new TestState { Value = 1 });

			Assert.IsTrue(worldState.IsAlive(0));
			Assert.IsFalse(worldState.IsAlive(1));

			worldState.SaveFrame();

			Assert.IsTrue(worldState.IsAlive(0));
			Assert.IsFalse(worldState.IsAlive(1));

			worldState.Rollback(1);

			Assert.IsFalse(worldState.IsAlive(0));
			Assert.IsFalse(worldState.IsAlive(1));
		}
	}
}
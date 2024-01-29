using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class WorldStateTests
	{
		private struct TestState
		{
			public int Value;
		}

		[Test]
		public void Delete_ShouldMakeNotAlive()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			worldState.Create(new TestState { Value = 1 });
			worldState.Create(new TestState { Value = 2 });
			worldState.Create(new TestState { Value = 3 });

			worldState.Delete(1);

			Assert.IsTrue(worldState.IsAlive(0));
			Assert.IsFalse(worldState.IsAlive(1));
			Assert.IsTrue(worldState.IsAlive(2));
		}
		
		[Test]
		public void DeleteThenCreate_ShouldRecycleIds()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			worldState.Create(new TestState { Value = 1 });
			int id2 = worldState.Create(new TestState { Value = 2 });
			worldState.Create(new TestState { Value = 3 });

			worldState.Delete(id2);
			
			int id = worldState.Create(new TestState { Value = 4 });
			
			Assert.AreEqual(id, id2);
		}

		[Test]
		public void Create_ShouldMakeStatesAlive()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			Assert.IsFalse(worldState.IsAlive(0));
			Assert.IsFalse(worldState.IsAlive(1));
			Assert.IsFalse(worldState.IsAlive(2));
			
			worldState.Create(new TestState { Value = 1 });
			worldState.Create(new TestState { Value = 2 });
			worldState.Create(new TestState { Value = 3 });

			Assert.IsTrue(worldState.IsAlive(0));
			Assert.IsTrue(worldState.IsAlive(1));
			Assert.IsTrue(worldState.IsAlive(2));
		}
		
		[Test]
		public void Create_ShouldInitializeData()
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
		public void State_WhenAffected_ShouldChangeState()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 2);

			worldState.Create(new TestState { Value = 1 });

			worldState.Get(0).Value = 2;

			Assert.AreEqual(worldState.Get(0).Value, 2);
		}

		[Test]
		public void SaveFrame_ShouldPreserveStates()
		{
			WorldState<TestState> worldState = new WorldState<TestState>(2, 4);

			worldState.Create(new TestState { Value = 1 });
			worldState.Create(new TestState { Value = 2 });
			worldState.Create(new TestState { Value = 3 });

			worldState.SaveFrame();

			Assert.AreEqual(worldState.Get(0).Value, 1);
			Assert.AreEqual(worldState.Get(1).Value, 2);
			Assert.AreEqual(worldState.Get(2).Value, 3);
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

			worldState.Create(new TestState { Value = 1 });
			worldState.SaveFrame();

			worldState.Get(0).Value = 2;
			worldState.Rollback(0);

			Assert.AreEqual(worldState.Get(0).Value, 1);
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
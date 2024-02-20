using NUnit.Framework;

namespace MassiveData.Tests
{
	[TestFixture]
	public class MassiveDataTests
	{
		private struct TestState
		{
			public int Value;
		}

		[Test]
		public void Delete_ShouldMakeNotAlive()
		{
			Massive<TestState> massive = new Massive<TestState>(2, 4);

			int id1 = massive.Create(new TestState { Value = 1 });
			int id2 = massive.Create(new TestState { Value = 2 });
			int id3 = massive.Create(new TestState { Value = 3 });

			massive.Delete(id2);

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsFalse(massive.IsAlive(id2));
			Assert.IsTrue(massive.IsAlive(id3));
		}

		[Test]
		public void DeleteThenCreate_ShouldRecycleIds()
		{
			Massive<TestState> massive = new Massive<TestState>(2, 4);

			int id1 = massive.Create(new TestState { Value = 1 });
			int id2 = massive.Create(new TestState { Value = 2 });
			int id3 = massive.Create(new TestState { Value = 3 });

			massive.Delete(id2);

			int id = massive.Create(new TestState { Value = 4 });

			Assert.AreEqual(id, id2);
		}

		[Test]
		public void Create_ShouldMakeStatesAlive()
		{
			Massive<TestState> massive = new Massive<TestState>(2, 4);

			Assert.IsFalse(massive.IsAlive(0));
			Assert.IsFalse(massive.IsAlive(1));
			Assert.IsFalse(massive.IsAlive(2));

			int id1 = massive.Create(new TestState { Value = 1 });
			int id2 = massive.Create(new TestState { Value = 2 });
			int id3 = massive.Create(new TestState { Value = 3 });

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsTrue(massive.IsAlive(id2));
			Assert.IsTrue(massive.IsAlive(id3));
		}

		[Test]
		public void Create_ShouldInitializeData()
		{
			Massive<TestState> massive = new Massive<TestState>(2, 4);

			int id1 = massive.Create(new TestState { Value = 1 });
			int id2 = massive.Create(new TestState { Value = 2 });
			int id3 = massive.Create(new TestState { Value = 3 });

			Assert.AreEqual(massive.Get(id1).Value, 1);
			Assert.AreEqual(massive.Get(id2).Value, 2);
			Assert.AreEqual(massive.Get(id3).Value, 3);
		}

		[Test]
		public void State_WhenAffected_ShouldChangeState()
		{
			Massive<TestState> massive = new Massive<TestState>(2, 2);

			int id1 = massive.Create(new TestState { Value = 1 });

			massive.Get(id1).Value = 2;

			Assert.AreEqual(massive.Get(id1).Value, 2);
		}

		[Test]
		public void SaveFrame_ShouldPreserveStates()
		{
			Massive<TestState> massive = new Massive<TestState>(2, 4);

			int id1 = massive.Create(new TestState { Value = 1 });
			int id2 = massive.Create(new TestState { Value = 2 });
			int id3 = massive.Create(new TestState { Value = 3 });

			massive.SaveFrame();

			Assert.AreEqual(massive.Get(id1).Value, 1);
			Assert.AreEqual(massive.Get(id2).Value, 2);
			Assert.AreEqual(massive.Get(id3).Value, 3);
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			Massive<TestState> massive = new Massive<TestState>(2, 2);

			int id1 = massive.Create(new TestState { Value = 1 });
			massive.SaveFrame();

			massive.Get(id1).Value = 2;
			massive.Rollback(0);

			Assert.AreEqual(massive.Get(id1).Value, 1);
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			Massive<TestState> massive = new Massive<TestState>(2, 2);

			massive.SaveFrame();

			int id1 = massive.Create(new TestState { Value = 1 });
			int id2 = massive.Create(new TestState { Value = 2 });
			massive.Delete(id2);

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsFalse(massive.IsAlive(id2));

			massive.SaveFrame();

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsFalse(massive.IsAlive(id2));

			massive.Rollback(1);

			Assert.IsFalse(massive.IsAlive(id1));
			Assert.IsFalse(massive.IsAlive(id2));
		}
	}
}
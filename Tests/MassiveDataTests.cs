using NUnit.Framework;

namespace Massive.Tests
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
			MassiveData<TestState> massiveData = new MassiveData<TestState>(2, 4);

			massiveData.Create(new TestState { Value = 1 });
			massiveData.Create(new TestState { Value = 2 });
			massiveData.Create(new TestState { Value = 3 });

			massiveData.Delete(1);

			Assert.IsTrue(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));
			Assert.IsTrue(massiveData.IsAlive(2));
		}
		
		[Test]
		public void DeleteThenCreate_ShouldRecycleIds()
		{
			MassiveData<TestState> massiveData = new MassiveData<TestState>(2, 4);

			massiveData.Create(new TestState { Value = 1 });
			int id2 = massiveData.Create(new TestState { Value = 2 });
			massiveData.Create(new TestState { Value = 3 });

			massiveData.Delete(id2);
			
			int id = massiveData.Create(new TestState { Value = 4 });
			
			Assert.AreEqual(id, id2);
		}

		[Test]
		public void Create_ShouldMakeStatesAlive()
		{
			MassiveData<TestState> massiveData = new MassiveData<TestState>(2, 4);

			Assert.IsFalse(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));
			Assert.IsFalse(massiveData.IsAlive(2));
			
			massiveData.Create(new TestState { Value = 1 });
			massiveData.Create(new TestState { Value = 2 });
			massiveData.Create(new TestState { Value = 3 });

			Assert.IsTrue(massiveData.IsAlive(0));
			Assert.IsTrue(massiveData.IsAlive(1));
			Assert.IsTrue(massiveData.IsAlive(2));
		}
		
		[Test]
		public void Create_ShouldInitializeData()
		{
			MassiveData<TestState> massiveData = new MassiveData<TestState>(2, 4);

			massiveData.Create(new TestState { Value = 1 });
			massiveData.Create(new TestState { Value = 2 });
			massiveData.Create(new TestState { Value = 3 });

			Assert.AreEqual(massiveData.Get(0).Value, 1);
			Assert.AreEqual(massiveData.Get(1).Value, 2);
			Assert.AreEqual(massiveData.Get(2).Value, 3);
		}

		[Test]
		public void State_WhenAffected_ShouldChangeState()
		{
			MassiveData<TestState> massiveData = new MassiveData<TestState>(2, 2);

			massiveData.Create(new TestState { Value = 1 });

			massiveData.Get(0).Value = 2;

			Assert.AreEqual(massiveData.Get(0).Value, 2);
		}

		[Test]
		public void SaveFrame_ShouldPreserveStates()
		{
			MassiveData<TestState> massiveData = new MassiveData<TestState>(2, 4);

			massiveData.Create(new TestState { Value = 1 });
			massiveData.Create(new TestState { Value = 2 });
			massiveData.Create(new TestState { Value = 3 });

			massiveData.SaveFrame();

			Assert.AreEqual(massiveData.Get(0).Value, 1);
			Assert.AreEqual(massiveData.Get(1).Value, 2);
			Assert.AreEqual(massiveData.Get(2).Value, 3);
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			MassiveData<TestState> massiveData = new MassiveData<TestState>(2, 2);

			massiveData.Create(new TestState { Value = 1 });
			massiveData.SaveFrame();

			massiveData.Get(0).Value = 2;
			massiveData.Rollback(0);

			Assert.AreEqual(massiveData.Get(0).Value, 1);
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			MassiveData<TestState> massiveData = new MassiveData<TestState>(2, 2);

			massiveData.SaveFrame();

			Assert.IsFalse(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));

			massiveData.Create(new TestState { Value = 1 });

			Assert.IsTrue(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));

			massiveData.SaveFrame();

			Assert.IsTrue(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));

			massiveData.Rollback(1);

			Assert.IsFalse(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));
		}
	}
}
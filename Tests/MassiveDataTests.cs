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
			var massiveData = new MassiveDataSet<TestState>(dataCapacity: 4, framesCapacity: 2);

			massiveData.Ensure(0, new TestState { Value = 1 });
			massiveData.Ensure(1, new TestState { Value = 2 });
			massiveData.Ensure(2, new TestState { Value = 3 });

			massiveData.Delete(1);

			Assert.IsTrue(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));
			Assert.IsTrue(massiveData.IsAlive(2));
		}

		[Test]
		public void Ensure_ShouldMakeStatesAlive()
		{
			var massiveData = new MassiveDataSet<TestState>(dataCapacity: 4, framesCapacity: 2);

			Assert.IsFalse(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));
			Assert.IsFalse(massiveData.IsAlive(2));

			massiveData.Ensure(0, new TestState { Value = 1 });
			massiveData.Ensure(1, new TestState { Value = 2 });
			massiveData.Ensure(2, new TestState { Value = 3 });

			Assert.IsTrue(massiveData.IsAlive(0));
			Assert.IsTrue(massiveData.IsAlive(1));
			Assert.IsTrue(massiveData.IsAlive(2));
		}

		[Test]
		public void Ensure_ShouldInitializeData()
		{
			var massiveData = new MassiveDataSet<TestState>(dataCapacity: 4, framesCapacity: 2);

			massiveData.Ensure(0, new TestState { Value = 1 });
			massiveData.Ensure(1, new TestState { Value = 2 });
			massiveData.Ensure(2, new TestState { Value = 3 });

			Assert.AreEqual(massiveData.Get(0).Value, 1);
			Assert.AreEqual(massiveData.Get(1).Value, 2);
			Assert.AreEqual(massiveData.Get(2).Value, 3);
		}

		[Test]
		public void State_WhenAffected_ShouldChangeState()
		{
			var massiveData = new MassiveDataSet<TestState>(dataCapacity: 2, framesCapacity: 2);

			massiveData.Ensure(0, new TestState { Value = 1 });

			massiveData.Get(0).Value = 2;

			Assert.AreEqual(massiveData.Get(0).Value, 2);
		}

		[Test]
		public void SaveFrame_ShouldPreserveStates()
		{
			var massiveData = new MassiveDataSet<TestState>(dataCapacity: 4, framesCapacity: 2);

			massiveData.Ensure(0, new TestState { Value = 1 });
			massiveData.Ensure(1, new TestState { Value = 2 });
			massiveData.Ensure(2, new TestState { Value = 3 });

			massiveData.SaveFrame();

			Assert.AreEqual(massiveData.Get(0).Value, 1);
			Assert.AreEqual(massiveData.Get(1).Value, 2);
			Assert.AreEqual(massiveData.Get(2).Value, 3);
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			var massiveData = new MassiveDataSet<TestState>(dataCapacity: 2, framesCapacity: 2);

			massiveData.Ensure(0, new TestState { Value = 1 });
			massiveData.SaveFrame();

			massiveData.Get(0).Value = 2;
			massiveData.Rollback(0);

			Assert.AreEqual(massiveData.Get(0).Value, 1);
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			var massiveData = new MassiveDataSet<TestState>(dataCapacity: 2, framesCapacity: 2);

			massiveData.SaveFrame();

			massiveData.Ensure(0, new TestState { Value = 1 });
			massiveData.Ensure(1, new TestState { Value = 2 });
			massiveData.Delete(1);

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
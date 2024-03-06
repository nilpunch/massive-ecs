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
			var massiveData = new MassiveDataSet<TestState>(2, 4);

			var id1 = massiveData.Ensure(0, new TestState { Value = 1 }).Id;
			var id2 = massiveData.Ensure(1, new TestState { Value = 2 }).Id;
			var id3 = massiveData.Ensure(2, new TestState { Value = 3 }).Id;

			massiveData.Delete(id2);

			Assert.IsTrue(massiveData.IsAlive(id1));
			Assert.IsFalse(massiveData.IsAlive(id2));
			Assert.IsTrue(massiveData.IsAlive(id3));
		}

		[Test]
		public void Create_ShouldMakeStatesAlive()
		{
			var massiveData = new MassiveDataSet<TestState>(2, 4);

			Assert.IsFalse(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));
			Assert.IsFalse(massiveData.IsAlive(2));

			var id1 = massiveData.Ensure(0, new TestState { Value = 1 }).Id;
			var id2 = massiveData.Ensure(1, new TestState { Value = 2 }).Id;
			var id3 = massiveData.Ensure(2, new TestState { Value = 3 }).Id;

			Assert.IsTrue(massiveData.IsAlive(id1));
			Assert.IsTrue(massiveData.IsAlive(id2));
			Assert.IsTrue(massiveData.IsAlive(id3));
		}

		[Test]
		public void Create_ShouldInitializeData()
		{
			var massiveData = new MassiveDataSet<TestState>(2, 4);

			var id1 = massiveData.Ensure(0, new TestState { Value = 1 }).Id;
			var id2 = massiveData.Ensure(1, new TestState { Value = 2 }).Id;
			var id3 = massiveData.Ensure(2, new TestState { Value = 3 }).Id;

			Assert.AreEqual(massiveData.Get(id1).Value, 1);
			Assert.AreEqual(massiveData.Get(id2).Value, 2);
			Assert.AreEqual(massiveData.Get(id3).Value, 3);
		}

		[Test]
		public void State_WhenAffected_ShouldChangeState()
		{
			var massiveData = new MassiveDataSet<TestState>(2, 2);

			var id1 = massiveData.Ensure(0, new TestState { Value = 1 }).Id;

			massiveData.Get(id1).Value = 2;

			Assert.AreEqual(massiveData.Get(id1).Value, 2);
		}

		[Test]
		public void SaveFrame_ShouldPreserveStates()
		{
			var massiveData = new MassiveDataSet<TestState>(2, 4);

			var id1 = massiveData.Ensure(0, new TestState { Value = 1 }).Id;
			var id2 = massiveData.Ensure(1, new TestState { Value = 2 }).Id;
			var id3 = massiveData.Ensure(2, new TestState { Value = 3 }).Id;

			massiveData.SaveFrame();

			Assert.AreEqual(massiveData.Get(id1).Value, 1);
			Assert.AreEqual(massiveData.Get(id2).Value, 2);
			Assert.AreEqual(massiveData.Get(id3).Value, 3);
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			var massiveData = new MassiveDataSet<TestState>(2, 2);

			var id1 = massiveData.Ensure(0, new TestState { Value = 1 }).Id;
			massiveData.SaveFrame();

			massiveData.Get(id1).Value = 2;
			massiveData.Rollback(0);

			Assert.AreEqual(massiveData.Get(id1).Value, 1);
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			var massiveData = new MassiveDataSet<TestState>(2, 2);

			massiveData.SaveFrame();

			var id1 = massiveData.Ensure(0, new TestState { Value = 1 }).Id;
			var id2 = massiveData.Ensure(1, new TestState { Value = 2 }).Id;
			massiveData.Delete(id2);

			Assert.IsTrue(massiveData.IsAlive(id1));
			Assert.IsFalse(massiveData.IsAlive(id2));

			massiveData.SaveFrame();

			Assert.IsTrue(massiveData.IsAlive(id1));
			Assert.IsFalse(massiveData.IsAlive(id2));

			massiveData.Rollback(1);

			Assert.IsFalse(massiveData.IsAlive(id1));
			Assert.IsFalse(massiveData.IsAlive(id2));
		}
	}
}
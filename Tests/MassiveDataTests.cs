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
			MassiveDataSet<TestState> massiveData = new MassiveDataSet<TestState>(2, 4);

			int id1 = massiveData.Create(new TestState { Value = 1 }).Id;
			int id2 = massiveData.Create(new TestState { Value = 2 }).Id;
			int id3 = massiveData.Create(new TestState { Value = 3 }).Id;

			massiveData.Delete(id2);

			Assert.IsTrue(massiveData.IsAlive(id1));
			Assert.IsFalse(massiveData.IsAlive(id2));
			Assert.IsTrue(massiveData.IsAlive(id3));
		}

		[Test]
		public void DeleteThenCreate_ShouldRecycleIds()
		{
			MassiveDataSet<TestState> massiveData = new MassiveDataSet<TestState>(2, 4);

			int id1 = massiveData.Create(new TestState { Value = 1 }).Id;
			int id2 = massiveData.Create(new TestState { Value = 2 }).Id;
			int id3 = massiveData.Create(new TestState { Value = 3 }).Id;

			massiveData.Delete(id2);

			int id = massiveData.Create(new TestState { Value = 4 }).Id;

			Assert.AreEqual(id, id2);
		}

		[Test]
		public void Create_ShouldMakeStatesAlive()
		{
			MassiveDataSet<TestState> massiveData = new MassiveDataSet<TestState>(2, 4);

			Assert.IsFalse(massiveData.IsAlive(0));
			Assert.IsFalse(massiveData.IsAlive(1));
			Assert.IsFalse(massiveData.IsAlive(2));

			int id1 = massiveData.Create(new TestState { Value = 1 }).Id;
			int id2 = massiveData.Create(new TestState { Value = 2 }).Id;
			int id3 = massiveData.Create(new TestState { Value = 3 }).Id;

			Assert.IsTrue(massiveData.IsAlive(id1));
			Assert.IsTrue(massiveData.IsAlive(id2));
			Assert.IsTrue(massiveData.IsAlive(id3));
		}

		[Test]
		public void Create_ShouldInitializeData()
		{
			MassiveDataSet<TestState> massiveData = new MassiveDataSet<TestState>(2, 4);

			int id1 = massiveData.Create(new TestState { Value = 1 }).Id;
			int id2 = massiveData.Create(new TestState { Value = 2 }).Id;
			int id3 = massiveData.Create(new TestState { Value = 3 }).Id;

			Assert.AreEqual(massiveData.Get(id1).Value, 1);
			Assert.AreEqual(massiveData.Get(id2).Value, 2);
			Assert.AreEqual(massiveData.Get(id3).Value, 3);
		}

		[Test]
		public void State_WhenAffected_ShouldChangeState()
		{
			MassiveDataSet<TestState> massiveData = new MassiveDataSet<TestState>(2, 2);

			int id1 = massiveData.Create(new TestState { Value = 1 }).Id;

			massiveData.Get(id1).Value = 2;

			Assert.AreEqual(massiveData.Get(id1).Value, 2);
		}

		[Test]
		public void SaveFrame_ShouldPreserveStates()
		{
			MassiveDataSet<TestState> massiveData = new MassiveDataSet<TestState>(2, 4);

			int id1 = massiveData.Create(new TestState { Value = 1 }).Id;
			int id2 = massiveData.Create(new TestState { Value = 2 }).Id;
			int id3 = massiveData.Create(new TestState { Value = 3 }).Id;

			massiveData.SaveFrame();

			Assert.AreEqual(massiveData.Get(id1).Value, 1);
			Assert.AreEqual(massiveData.Get(id2).Value, 2);
			Assert.AreEqual(massiveData.Get(id3).Value, 3);
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			MassiveDataSet<TestState> massiveData = new MassiveDataSet<TestState>(2, 2);

			int id1 = massiveData.Create(new TestState { Value = 1 }).Id;
			massiveData.SaveFrame();

			massiveData.Get(id1).Value = 2;
			massiveData.Rollback(0);

			Assert.AreEqual(massiveData.Get(id1).Value, 1);
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			MassiveDataSet<TestState> massiveData = new MassiveDataSet<TestState>(2, 2);

			massiveData.SaveFrame();

			int id1 = massiveData.Create(new TestState { Value = 1 }).Id;
			int id2 = massiveData.Create(new TestState { Value = 2 }).Id;
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
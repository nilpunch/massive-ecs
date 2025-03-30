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
			var massiveData = new MassiveDataSet<TestState>(framesCapacity: 2);

			massiveData.Set(0, new TestState { Value = 1 });
			massiveData.Set(1, new TestState { Value = 2 });
			massiveData.Set(2, new TestState { Value = 3 });

			massiveData.Remove(1);

			NUnit.Framework.Assert.IsTrue(massiveData.Has(0));
			NUnit.Framework.Assert.IsFalse(massiveData.Has(1));
			NUnit.Framework.Assert.IsTrue(massiveData.Has(2));
		}

		[Test]
		public void Ensure_ShouldMakeStatesAlive()
		{
			var massiveData = new MassiveDataSet<TestState>(framesCapacity: 2);

			NUnit.Framework.Assert.IsFalse(massiveData.Has(0));
			NUnit.Framework.Assert.IsFalse(massiveData.Has(1));
			NUnit.Framework.Assert.IsFalse(massiveData.Has(2));

			massiveData.Set(0, new TestState { Value = 1 });
			massiveData.Set(1, new TestState { Value = 2 });
			massiveData.Set(2, new TestState { Value = 3 });

			NUnit.Framework.Assert.IsTrue(massiveData.Has(0));
			NUnit.Framework.Assert.IsTrue(massiveData.Has(1));
			NUnit.Framework.Assert.IsTrue(massiveData.Has(2));
		}

		[Test]
		public void Ensure_ShouldInitializeData()
		{
			var massiveData = new MassiveDataSet<TestState>(framesCapacity: 2);

			massiveData.Set(0, new TestState { Value = 1 });
			massiveData.Set(1, new TestState { Value = 2 });
			massiveData.Set(2, new TestState { Value = 3 });

			NUnit.Framework.Assert.AreEqual(massiveData.Get(0).Value, 1);
			NUnit.Framework.Assert.AreEqual(massiveData.Get(1).Value, 2);
			NUnit.Framework.Assert.AreEqual(massiveData.Get(2).Value, 3);
		}

		[Test]
		public void State_WhenAffected_ShouldChangeState()
		{
			var massiveData = new MassiveDataSet<TestState>(framesCapacity: 2);

			massiveData.Set(0, new TestState { Value = 1 });

			massiveData.Get(0).Value = 2;

			NUnit.Framework.Assert.AreEqual(massiveData.Get(0).Value, 2);
		}

		[Test]
		public void SaveFrame_ShouldPreserveStates()
		{
			var massiveData = new MassiveDataSet<TestState>(framesCapacity: 2);

			massiveData.Set(0, new TestState { Value = 1 });
			massiveData.Set(1, new TestState { Value = 2 });
			massiveData.Set(2, new TestState { Value = 3 });

			massiveData.SaveFrame();

			NUnit.Framework.Assert.AreEqual(massiveData.Get(0).Value, 1);
			NUnit.Framework.Assert.AreEqual(massiveData.Get(1).Value, 2);
			NUnit.Framework.Assert.AreEqual(massiveData.Get(2).Value, 3);
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			var massiveData = new MassiveDataSet<TestState>(framesCapacity: 2);

			massiveData.Set(0, new TestState { Value = 1 });
			massiveData.SaveFrame();

			massiveData.Get(0).Value = 2;
			massiveData.Rollback(0);

			NUnit.Framework.Assert.AreEqual(massiveData.Get(0).Value, 1);
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			var massiveData = new MassiveDataSet<TestState>(framesCapacity: 2);

			massiveData.SaveFrame();

			massiveData.Set(0, new TestState { Value = 1 });
			massiveData.Set(1, new TestState { Value = 2 });
			massiveData.Remove(1);

			NUnit.Framework.Assert.IsTrue(massiveData.Has(0));
			NUnit.Framework.Assert.IsFalse(massiveData.Has(1));

			massiveData.SaveFrame();

			NUnit.Framework.Assert.IsTrue(massiveData.Has(0));
			NUnit.Framework.Assert.IsFalse(massiveData.Has(1));

			massiveData.Rollback(1);

			NUnit.Framework.Assert.IsFalse(massiveData.Has(0));
			NUnit.Framework.Assert.IsFalse(massiveData.Has(1));
		}
	}
}

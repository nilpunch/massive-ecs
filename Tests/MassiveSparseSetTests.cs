using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class MassiveSparseSetTests
	{
		[Test]
		public void Ensure_ShouldMakeAlive()
		{
			var massive = new MassiveSparseSet(dataCapacity: 4, framesCapacity: 2);

			var id = 2;

			Assert.IsFalse(massive.IsAlive(id));

			massive.Ensure(id);

			Assert.IsTrue(massive.IsAlive(id));
		}

		[Test]
		public void Delete_ShouldMakeNotAlive()
		{
			var massive = new MassiveSparseSet(dataCapacity: 4, framesCapacity: 2);

			massive.Ensure(0);
			massive.Ensure(1);
			massive.Ensure(2);

			massive.Delete(1);

			Assert.IsTrue(massive.IsAlive(0));
			Assert.IsFalse(massive.IsAlive(1));
			Assert.IsTrue(massive.IsAlive(2));
		}

		[Test]
		public void Ensure_ShouldMakeStatesAlive()
		{
			var massive = new MassiveSparseSet(dataCapacity: 4, framesCapacity: 2);

			Assert.IsFalse(massive.IsAlive(0));
			Assert.IsFalse(massive.IsAlive(1));
			Assert.IsFalse(massive.IsAlive(2));

			massive.Ensure(0);
			massive.Ensure(1);
			massive.Ensure(2);

			Assert.IsTrue(massive.IsAlive(0));
			Assert.IsTrue(massive.IsAlive(1));
			Assert.IsTrue(massive.IsAlive(2));
		}

		[Test]
		public void SaveFrame_ShouldPreserveLiveliness()
		{
			var massive = new MassiveSparseSet(dataCapacity: 4, framesCapacity: 2);

			massive.Ensure(0);
			massive.Ensure(1);
			massive.Ensure(2);

			massive.SaveFrame();

			Assert.IsTrue(massive.IsAlive(0));
			Assert.IsTrue(massive.IsAlive(1));
			Assert.IsTrue(massive.IsAlive(2));
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			var massive = new MassiveSparseSet(dataCapacity: 2, framesCapacity: 2);

			massive.Ensure(0);

			Assert.IsTrue(massive.IsAlive(0));

			massive.SaveFrame();

			massive.Delete(0);

			Assert.IsFalse(massive.IsAlive(0));

			massive.Rollback(0);

			Assert.IsTrue(massive.IsAlive(0));
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			var massive = new MassiveSparseSet(dataCapacity: 2, framesCapacity: 2);

			massive.SaveFrame();

			massive.Ensure(0);
			massive.Ensure(1);
			massive.Delete(1);

			Assert.IsTrue(massive.IsAlive(0));
			Assert.IsFalse(massive.IsAlive(1));

			massive.SaveFrame();

			Assert.IsTrue(massive.IsAlive(0));
			Assert.IsFalse(massive.IsAlive(1));

			massive.Rollback(1);

			Assert.IsFalse(massive.IsAlive(0));
			Assert.IsFalse(massive.IsAlive(1));
		}
	}
}
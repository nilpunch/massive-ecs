using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class MassiveSparseSetTests
	{
		[Test]
		public void Ensure_ShouldMakeAlive()
		{
			var massive = new MassiveSparseSet(2, 4);

			var id = 2;

			Assert.IsFalse(massive.IsAlive(id));

			massive.Ensure(id);

			Assert.IsTrue(massive.IsAlive(id));
		}

		[Test]
		public void Delete_ShouldMakeNotAlive()
		{
			var massive = new MassiveSparseSet(2, 4);

			var id1 = massive.Ensure(0).Id;
			var id2 = massive.Ensure(1).Id;
			var id3 = massive.Ensure(2).Id;

			massive.Delete(id2);

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsFalse(massive.IsAlive(id2));
			Assert.IsTrue(massive.IsAlive(id3));
		}

		[Test]
		public void Ensure_ShouldMakeStatesAlive()
		{
			var massive = new MassiveSparseSet(2, 4);

			Assert.IsFalse(massive.IsAlive(0));
			Assert.IsFalse(massive.IsAlive(1));
			Assert.IsFalse(massive.IsAlive(2));

			var id1 = massive.Ensure(0).Id;
			var id2 = massive.Ensure(1).Id;
			var id3 = massive.Ensure(2).Id;

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsTrue(massive.IsAlive(id2));
			Assert.IsTrue(massive.IsAlive(id3));
		}

		[Test]
		public void SaveFrame_ShouldPreserveLiveliness()
		{
			var massive = new MassiveSparseSet(2, 4);

			var id1 = massive.Ensure(0).Id;
			var id2 = massive.Ensure(1).Id;
			var id3 = massive.Ensure(2).Id;

			massive.SaveFrame();

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsTrue(massive.IsAlive(id2));
			Assert.IsTrue(massive.IsAlive(id3));
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			var massive = new MassiveSparseSet(2, 2);

			var id1 = massive.Ensure(0).Id;

			Assert.IsTrue(massive.IsAlive(id1));

			massive.SaveFrame();

			massive.Delete(id1);

			Assert.IsFalse(massive.IsAlive(id1));

			massive.Rollback(0);

			Assert.IsTrue(massive.IsAlive(id1));
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			var massive = new MassiveSparseSet(2, 2);

			massive.SaveFrame();

			var id1 = massive.Ensure(0).Id;
			var id2 = massive.Ensure(1).Id;
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
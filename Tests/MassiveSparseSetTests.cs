using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class MassiveSparseSetTests
	{
		[Test]
		public void Ensure_ShouldMakeAlive()
		{
			MassiveSparseSet massive = new MassiveSparseSet(2, 4);

			int id = 2;

			Assert.IsFalse(massive.IsAlive(id));

			massive.Ensure(id);

			Assert.IsTrue(massive.IsAlive(id));
		}

		[Test]
		public void Delete_ShouldMakeNotAlive()
		{
			MassiveSparseSet massive = new MassiveSparseSet(2, 4);

			int id1 = massive.Create().Id;
			int id2 = massive.Create().Id;
			int id3 = massive.Create().Id;

			massive.Delete(id2);

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsFalse(massive.IsAlive(id2));
			Assert.IsTrue(massive.IsAlive(id3));
		}

		[Test]
		public void DeleteThenCreate_ShouldRecycleIds()
		{
			MassiveSparseSet massive = new MassiveSparseSet(2, 4);

			int id1 = massive.Create().Id;
			int id2 = massive.Create().Id;
			int id3 = massive.Create().Id;

			massive.Delete(id2);

			int id = massive.Create().Id;

			Assert.AreEqual(id, id2);
		}

		[Test]
		public void Create_ShouldMakeStatesAlive()
		{
			MassiveSparseSet massive = new MassiveSparseSet(2, 4);

			Assert.IsFalse(massive.IsAlive(0));
			Assert.IsFalse(massive.IsAlive(1));
			Assert.IsFalse(massive.IsAlive(2));

			int id1 = massive.Create().Id;
			int id2 = massive.Create().Id;
			int id3 = massive.Create().Id;

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsTrue(massive.IsAlive(id2));
			Assert.IsTrue(massive.IsAlive(id3));
		}

		[Test]
		public void SaveFrame_ShouldPreserveLiveliness()
		{
			MassiveSparseSet massive = new MassiveSparseSet(2, 4);

			int id1 = massive.Create().Id;
			int id2 = massive.Create().Id;
			int id3 = massive.Create().Id;

			massive.SaveFrame();

			Assert.IsTrue(massive.IsAlive(id1));
			Assert.IsTrue(massive.IsAlive(id2));
			Assert.IsTrue(massive.IsAlive(id3));
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			MassiveSparseSet massive = new MassiveSparseSet(2, 2);

			int id1 = massive.Create().Id;

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
			MassiveSparseSet massive = new MassiveSparseSet(2, 2);

			massive.SaveFrame();

			int id1 = massive.Create().Id;
			int id2 = massive.Create().Id;
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
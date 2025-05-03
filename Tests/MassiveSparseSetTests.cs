using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class MassiveSparseSetTests
	{
		[Test]
		public void Ensure_ShouldMakeAlive()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			var id = 2;

			Assert.IsFalse(massive.Has(id));

			massive.Add(id);

			Assert.IsTrue(massive.Has(id));
		}

		[Test]
		public void Delete_ShouldMakeNotAlive()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			massive.Add(0);
			massive.Add(1);
			massive.Add(2);

			massive.Remove(1);

			Assert.IsTrue(massive.Has(0));
			Assert.IsFalse(massive.Has(1));
			Assert.IsTrue(massive.Has(2));
		}

		[Test]
		public void Ensure_ShouldMakeStatesAlive()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			Assert.IsFalse(massive.Has(0));
			Assert.IsFalse(massive.Has(1));
			Assert.IsFalse(massive.Has(2));

			massive.Add(0);
			massive.Add(1);
			massive.Add(2);

			Assert.IsTrue(massive.Has(0));
			Assert.IsTrue(massive.Has(1));
			Assert.IsTrue(massive.Has(2));
		}

		[Test]
		public void SaveFrame_ShouldPreserveLiveliness()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			massive.Add(0);
			massive.Add(1);
			massive.Add(2);

			massive.SaveFrame();

			Assert.IsTrue(massive.Has(0));
			Assert.IsTrue(massive.Has(1));
			Assert.IsTrue(massive.Has(2));
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			massive.Add(0);

			Assert.IsTrue(massive.Has(0));

			massive.SaveFrame();

			massive.Remove(0);

			Assert.IsFalse(massive.Has(0));

			massive.Rollback(0);

			Assert.IsTrue(massive.Has(0));
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			massive.SaveFrame();

			massive.Add(0);
			massive.Add(1);
			massive.Remove(1);

			Assert.IsTrue(massive.Has(0));
			Assert.IsFalse(massive.Has(1));

			massive.SaveFrame();

			Assert.IsTrue(massive.Has(0));
			Assert.IsFalse(massive.Has(1));

			massive.Rollback(1);

			Assert.IsFalse(massive.Has(0));
			Assert.IsFalse(massive.Has(1));
		}
	}
}

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

			NUnit.Framework.Assert.IsFalse(massive.Has(id));

			massive.Add(id);

			NUnit.Framework.Assert.IsTrue(massive.Has(id));
		}

		[Test]
		public void Delete_ShouldMakeNotAlive()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			massive.Add(0);
			massive.Add(1);
			massive.Add(2);

			massive.Remove(1);

			NUnit.Framework.Assert.IsTrue(massive.Has(0));
			NUnit.Framework.Assert.IsFalse(massive.Has(1));
			NUnit.Framework.Assert.IsTrue(massive.Has(2));
		}

		[Test]
		public void Ensure_ShouldMakeStatesAlive()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			NUnit.Framework.Assert.IsFalse(massive.Has(0));
			NUnit.Framework.Assert.IsFalse(massive.Has(1));
			NUnit.Framework.Assert.IsFalse(massive.Has(2));

			massive.Add(0);
			massive.Add(1);
			massive.Add(2);

			NUnit.Framework.Assert.IsTrue(massive.Has(0));
			NUnit.Framework.Assert.IsTrue(massive.Has(1));
			NUnit.Framework.Assert.IsTrue(massive.Has(2));
		}

		[Test]
		public void SaveFrame_ShouldPreserveLiveliness()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			massive.Add(0);
			massive.Add(1);
			massive.Add(2);

			massive.SaveFrame();

			NUnit.Framework.Assert.IsTrue(massive.Has(0));
			NUnit.Framework.Assert.IsTrue(massive.Has(1));
			NUnit.Framework.Assert.IsTrue(massive.Has(2));
		}

		[Test]
		public void RollbackZero_ShouldResetCurrentFrameChanges()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			massive.Add(0);

			NUnit.Framework.Assert.IsTrue(massive.Has(0));

			massive.SaveFrame();

			massive.Remove(0);

			NUnit.Framework.Assert.IsFalse(massive.Has(0));

			massive.Rollback(0);

			NUnit.Framework.Assert.IsTrue(massive.Has(0));
		}

		[Test]
		public void IsAlive_ShouldWorkCorrectWithRollback()
		{
			var massive = new MassiveSparseSet(framesCapacity: 2);

			massive.SaveFrame();

			massive.Add(0);
			massive.Add(1);
			massive.Remove(1);

			NUnit.Framework.Assert.IsTrue(massive.Has(0));
			NUnit.Framework.Assert.IsFalse(massive.Has(1));

			massive.SaveFrame();

			NUnit.Framework.Assert.IsTrue(massive.Has(0));
			NUnit.Framework.Assert.IsFalse(massive.Has(1));

			massive.Rollback(1);

			NUnit.Framework.Assert.IsFalse(massive.Has(0));
			NUnit.Framework.Assert.IsFalse(massive.Has(1));
		}
	}
}

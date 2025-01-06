using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class SparseSetTests
	{
		public int[] IdsToAssign =
		{
			0, 1, 2, 3, 4, 5, 6, 7, 20, 21, 22, 23
		};

		public int[] IdsToUnassign =
		{
			0, 1, 3, 20, 21, 23
		};

		[Test]
		public void WhenCompact_AndThereIsHoles_ThenShouldRemoveHoles()
		{
			// Arrange.
			var sparseSet = new SparseSet(packing: Packing.WithHoles);
			foreach (var id in IdsToAssign)
				sparseSet.Assign(id);
			foreach (var id in IdsToUnassign)
				sparseSet.Unassign(id);

			// Act.
			sparseSet.Compact();

			// Assert.
			int remainIdsCount = IdsToAssign.Length - IdsToUnassign.Length;
			Assert.AreEqual(remainIdsCount, sparseSet.Count);
		}

		[TestCase(-1)]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void WhenIsAssigned_AndNotAssigned_ThenReturnFalse(int id)
		{
			var sparseSet = new SparseSet();

			var isAssigned = sparseSet.IsAssigned(id);

			Assert.IsFalse(isAssigned);
		}

		[TestCase(-1)]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void WhenGetIndexOrInvalid_AndNotAssigned_ThenReturnNegativeIndex(int id)
		{
			var sparseSet = new SparseSet();

			var index = sparseSet.GetIndexOrNegative(id);

			Assert.IsTrue(index < 0);
		}

		[TestCase(-2)]
		[TestCase(-1)]
		public void WhenAssigned_AndNegativeIndex_ThenDoNotAssign(int id)
		{
			var sparseSet = new SparseSet();

			sparseSet.Assign(id);
			var isAssigned = sparseSet.IsAssigned(id);

			Assert.IsFalse(isAssigned);
		}
	}
}

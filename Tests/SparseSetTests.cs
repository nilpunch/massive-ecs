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
			// Arrange
			var sparseSet = new SparseSet(packingMode: PackingMode.WithHoles);
			foreach (var id in IdsToAssign)
				sparseSet.Assign(id);
			foreach (var id in IdsToUnassign)
				sparseSet.Unassign(id);

			// Act
			sparseSet.Compact();

			// Assert
			int remainIdsCount = IdsToAssign.Length - IdsToUnassign.Length;
			Assert.AreEqual(remainIdsCount, sparseSet.Count);
		}
	}
}

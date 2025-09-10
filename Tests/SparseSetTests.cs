﻿using System;
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

		public int[] IdsToRemove =
		{
			0, 1, 3, 20, 21, 23
		};

		[TestCase(-1)]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void WhenIsAssigned_AndNotAssigned_ThenReturnFalse(int id)
		{
			var sparseSet = new SparseSet();

			var isAssigned = sparseSet.Has(id);

			Assert.IsFalse(isAssigned);
		}

		[TestCase(-2)]
		[TestCase(-1)]
		public void WhenAssigned_AndNegativeIndex_ThenThrow(int id)
		{
			var sparseSet = new SparseSet();

			Assert.Throws<NegativeArgumentException>(() =>
			{
				sparseSet.Add(id);
			});
		}
	}
}

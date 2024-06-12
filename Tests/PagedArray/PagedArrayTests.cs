using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class PagedArrayTests
	{
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(Constants.DefaultPageSize)]
		[TestCase(Constants.DefaultPageSize + 1)]
		[TestCase(Constants.DefaultPageSize - 1)]
		[TestCase(Constants.DefaultPageSize * 2)]
		[TestCase(Constants.DefaultPageSize * 2 + 1)]
		[TestCase(Constants.DefaultPageSize * 2 - 1)]
		public void PagedSpan_ShouldIterateOverAllElements(int length)
		{
			var packedArray = new PagedArray<int>();

			for (int i = 0; i < length; i++)
			{
				packedArray.EnsurePageForIndex(i);
				packedArray[i] = i;
			}

			var span = new PagedSpan<int>(packedArray, length);

			int iterationsAmount = 0;

			foreach (var i in span)
			{
				iterationsAmount += 1;
				Assert.AreEqual(length - iterationsAmount, i);
			}

			Assert.AreEqual(length, iterationsAmount);
		}
	}
}

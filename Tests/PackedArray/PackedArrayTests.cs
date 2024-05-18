using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class PackedArrayTests
	{
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(Constants.PageSize)]
		[TestCase(Constants.PageSize + 1)]
		[TestCase(Constants.PageSize - 1)]
		[TestCase(Constants.PageSize * 2)]
		[TestCase(Constants.PageSize * 2 + 1)]
		[TestCase(Constants.PageSize * 2 - 1)]
		public void PagedSpan_ShouldIterateOverAllElements(int length)
		{
			var packedArray = new PagedArray<int>();

			for (int i = 0; i < length; i++)
			{
				packedArray.GetSafe(i) = i;
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

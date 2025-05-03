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
		public void PageSequence_ShouldNotReturnZeroLengthPage(int length)
		{
			var sequence = new PageSequence(Constants.DefaultPageSize, length);

			foreach (var page in sequence)
			{
				Assert.AreNotEqual(0, page.Length);
			}
		}
		
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
			var pagedArray = new PagedArray<int>(Constants.DefaultPageSize);

			for (int i = 0; i < length; i++)
			{
				pagedArray.EnsurePageAt(i);
				pagedArray[i] = i;
			}

			var span = new PagedSpan<int>(pagedArray, length);

			int iterationsAmount = 0;

			foreach (var i in span)
			{
				iterationsAmount += 1;
				Assert.AreEqual(length - iterationsAmount, i);
			}

			Assert.AreEqual(length, iterationsAmount);
		}

		[TestCase(Constants.DefaultPageSize, ExpectedResult = 0)]
		[TestCase(Constants.DefaultPageSize + 1, ExpectedResult = 1)]
		[TestCase(Constants.DefaultPageSize - 1, ExpectedResult = Constants.DefaultPageSize - 1)]
		[TestCase(Constants.DefaultPageSize * 2, ExpectedResult = 0)]
		[TestCase(Constants.DefaultPageSize * 2 + 1, ExpectedResult = 1)]
		[TestCase(Constants.DefaultPageSize * 2 - 1, ExpectedResult = Constants.DefaultPageSize - 1)]
		public int FastMod_ShouldReturnCorrectResult(int value)
		{
			return MathUtils.FastMod(value, Constants.DefaultPageSize);
		}

		[TestCase(Constants.DefaultPageSize, ExpectedResult = 1)]
		[TestCase(Constants.DefaultPageSize + 1, ExpectedResult = 1)]
		[TestCase(Constants.DefaultPageSize - 1, ExpectedResult = 0)]
		[TestCase(Constants.DefaultPageSize * 2, ExpectedResult = 2)]
		[TestCase(Constants.DefaultPageSize * 2 + 1, ExpectedResult = 2)]
		[TestCase(Constants.DefaultPageSize * 2 - 1, ExpectedResult = 1)]
		public int FastDivPow_ShouldReturnCorrectResult(int value)
		{
			int pow = MathUtils.FastLog2(Constants.DefaultPageSize);
			
			return MathUtils.FastPowDiv(value, pow);
		}
	}
}

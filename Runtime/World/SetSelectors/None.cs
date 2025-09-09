using System;

namespace Massive
{
	public class None : IIncludeSelector, IExcludeSelector
	{
		public BitSet[] Select(BitSets bitSets) => Array.Empty<BitSet>();
	}
}

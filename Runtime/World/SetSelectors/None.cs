using System;

namespace Massive
{
	public class None : IIncludeSelector, IExcludeSelector
	{
		public BitSet[] Select(Sets sets) => Array.Empty<BitSet>();
	}
}

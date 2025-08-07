using System;

namespace Massive
{
	public class None : IIncludeSelector, IExcludeSelector
	{
		public SparseSet[] Select(Sets sets, bool negative) => Array.Empty<SparseSet>();
	}
}

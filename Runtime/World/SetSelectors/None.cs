using System;

namespace Massive
{
	public class None : IIncludeSelector, IExcludeSelector
	{
		public SparseSet[] Select(Sets sets) => Array.Empty<SparseSet>();
	}
}

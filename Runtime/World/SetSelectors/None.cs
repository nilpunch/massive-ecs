using System;

namespace Massive
{
	public class None : IIncludeSelector, IExcludeSelector
	{
		public SparseSet[] Select(SetRegistry setRegistry) => Array.Empty<SparseSet>();
	}
}

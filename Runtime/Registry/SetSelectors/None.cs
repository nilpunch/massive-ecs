using System;

namespace Massive
{
	public class None : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		public SparseSet[] Select(SetRegistry setRegistry) => Array.Empty<SparseSet>();
	}
}

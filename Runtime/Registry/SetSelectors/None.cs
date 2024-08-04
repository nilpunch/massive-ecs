using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class None : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet[] Select(SetRegistry setRegistry) => Array.Empty<SparseSet>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public SparseSet[] SelectReadOnly(SetRegistry setRegistry) => Array.Empty<SparseSet>();
	}
}

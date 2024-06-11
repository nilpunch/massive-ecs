using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class None : IOwnSelector, IIncludeSelector, IExcludeSelector
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISet[] Select(SetRegistry setRegistry) => Array.Empty<ISet>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public IReadOnlySet[] SelectReadOnly(SetRegistry setRegistry) => Array.Empty<IReadOnlySet>();
	}
}

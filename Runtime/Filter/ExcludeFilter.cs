using System;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class ExcludeFilter : IFilter
	{
		private readonly SparseSet[] _exclude;

		public ExcludeFilter(SparseSet[] exclude)
		{
			_exclude = exclude;
		}

		public SparseSet[] Include => Array.Empty<SparseSet>();

		public bool ContainsId(int id)
		{
			return id >= 0 && SetHelpers.NotAssignedInAll(id, _exclude);
		}
	}
}

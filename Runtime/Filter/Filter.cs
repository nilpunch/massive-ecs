using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class Filter
	{
		public static Filter Empty { get; } = new Filter(Array.Empty<SparseSet>(), Array.Empty<SparseSet>());

		private readonly SparseSet[] _exclude;
		private readonly SparseSet[] _include;

		public Filter(SparseSet[] include, SparseSet[] exclude)
		{
			_include = include;
			_exclude = exclude;

			for (int i = 0; i < _exclude.Length; i++)
			{
				if (_include.Contains(_exclude[i]))
				{
					throw new Exception("Conflicting include and exclude filter!");
				}
			}
		}

		public SparseSet[] Include => _include;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			if (_include.Length == 0 && _exclude.Length == 0)
			{
				return id >= 0;
			}
			else if (_include.Length == 0)
			{
				return id >= 0 && SetHelpers.NotAssignedInAll(id, _exclude);
			}
			else if (_exclude.Length == 0)
			{
				return id >= 0 && SetHelpers.AssignedInAll(id, _exclude);
			}
			else
			{
				return id >= 0 && SetHelpers.AssignedInAll(id, _exclude) && SetHelpers.NotAssignedInAll(id, _exclude);
			}
		}
	}
}

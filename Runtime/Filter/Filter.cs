using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks | Option.ArrayBoundsChecks, false)]
	public class Filter
	{
		public static Filter Empty { get; } = new Filter(Array.Empty<SparseSet>(), Array.Empty<SparseSet>());

		public SparseSet[] Include { get; }
		public SparseSet[] Exclude { get; }

		public Filter(SparseSet[] include, SparseSet[] exclude)
		{
			Include = include;
			Exclude = exclude;

			for (int i = 0; i < Exclude.Length; i++)
			{
				if (Include.Contains(Exclude[i]))
				{
					throw new Exception("Conflicting include and exclude filter!");
				}
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			return id >= 0 && (Include.Length == 0 || SetHelpers.AssignedInAll(id, Include)) && (Exclude.Length == 0 || SetHelpers.NotAssignedInAll(id, Exclude));
		}
	}
}

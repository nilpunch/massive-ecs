#if !MASSIVE_DISABLE_ASSERT
#define MASSIVE_ASSERT
#endif

using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class Filter
	{
		public int IncludedCount { get; protected set; }
		public int ExcludedCount { get; protected set; }

		public SparseSet[] Included { get; protected set; }
		public SparseSet[] Excluded { get; protected set; }
		public Mask Mask { get; protected set; }

		public Filter(Masks masks)
			: this(Array.Empty<SparseSet>(), Array.Empty<SparseSet>(), masks)
		{
		}

		public Filter(SparseSet[] included, SparseSet[] excluded, Masks masks)
		{
			ConflictingFilterException.ThrowIfHasConflicts(included, excluded);

			Mask = Mask.New(masks);

			foreach (var include in included)
			{
				Mask.Include(include.ComponentId);
			}

			foreach (var exclude in excluded)
			{
				Mask.Exclude(exclude.ComponentId);
			}

			Included = included;
			Excluded = excluded;
			IncludedCount = included.Length;
			ExcludedCount = excluded.Length;
		}
	}
}

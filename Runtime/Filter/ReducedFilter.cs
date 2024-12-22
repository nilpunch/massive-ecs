using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct ReducedFilter
	{
		public SparseSet Reduced { get; }

		public SparseSet[] Included { get; }
		public int IncludedLength { get; }

		public SparseSet[] Excluded { get; }
		public int ExcludedLength { get; }

		public ReducedFilter(SparseSet[] included, int includedLength, SparseSet[] excluded, int excludedLength, SparseSet reduced)
		{
			Included = included;
			IncludedLength = includedLength;
			Excluded = excluded;
			ExcludedLength = excludedLength;
			Reduced = reduced;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			return (SetUtils.NonNegativeIfAssignedInAll(id, Included, IncludedLength)
				| ~SetUtils.NegativeIfNotAssignedInAll(id, Excluded, ExcludedLength)) >= 0;
		}
	}
}

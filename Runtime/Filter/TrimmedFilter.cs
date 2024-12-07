using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public readonly struct TrimmedFilter : IDisposable
	{
		public SparseSet[] Included { get; }
		public int IncludedLength { get; }
		public bool IsRented { get; }

		public SparseSet[] Excluded { get; }
		public int ExcludedLength { get; }

		public TrimmedFilter(SparseSet[] included, int includedLength, bool isRented, SparseSet[] excluded, int excludedLength)
		{
			Included = included;
			IncludedLength = includedLength;
			IsRented = isRented;
			Excluded = excluded;
			ExcludedLength = excludedLength;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			return (SetUtils.NonNegativeIfAssignedInAll(id, Included, IncludedLength)
				| ~SetUtils.NegativeIfNotAssignedInAll(id, Excluded, ExcludedLength)) >= 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Dispose()
		{
			if (IsRented)
			{
				ArrayPool<SparseSet>.Shared.Return(Included);
			}
		}
	}
}

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
		public bool IncludedRented { get; }

		public SparseSet[] Excluded { get; }
		public int ExcludedLength { get; }

		public TrimmedFilter(SparseSet[] included, int includedLength, bool includedRented, SparseSet[] excluded)
		{
			Included = included;
			IncludedLength = includedLength;
			IncludedRented = includedRented;
			Excluded = excluded;
			ExcludedLength = excluded.Length;
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
			if (IncludedRented)
			{
				ArrayPool<SparseSet>.Shared.Return(Included);
			}
		}
	}
}

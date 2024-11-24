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
		public bool IsRented { get; }
		public int IncludedLength { get; }
		public int ExcludedLength { get; }
		public SparseSet[] Included { get; }
		public SparseSet[] Excluded { get; }

		public TrimmedFilter(SparseSet[] included, int includedLength, bool isRented, SparseSet[] excluded)
		{
			Included = included;
			IncludedLength = includedLength;
			IsRented = isRented;
			Excluded = excluded;
			ExcludedLength = excluded.Length;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool ContainsId(int id)
		{
			return (SetUtils.NonNegativeIfAssignedInAll(id, Included, IncludedLength)
				| ~SetUtils.NegativeIfNotAssignedInAll(id, Excluded, ExcludedLength)) >= 0;
		}

		public void Dispose()
		{
			if (IsRented)
			{
				ArrayPool<SparseSet>.Shared.Return(Included);
			}
		}
	}
}

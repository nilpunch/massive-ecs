using System;
using System.Runtime.CompilerServices;

namespace Massive
{
	public partial class Sets
	{
		public BitSet[][] SelectionLookup { get; private set; } = Array.Empty<BitSet[]>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public BitSet[] Select<TSelector>()
			where TSelector : ISetSelector, new()
		{
			var info = TypeId<TSelector>.Info;

			EnsureSelectionLookupAt(info.Index);
			var candidate = SelectionLookup[info.Index];

			if (candidate != null)
			{
				return candidate;
			}

			var sets = new TSelector().Select(this);
			
			ConflictingFilterException.ThrowIfHasDuplicates(sets);

			SelectionLookup[info.Index] = sets;

			return sets;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void EnsureSelectionLookupAt(int index)
		{
			if (index >= SelectionLookup.Length)
			{
				SelectionLookup = SelectionLookup.ResizeToNextPowOf2(index + 1);
			}
		}
	}
}

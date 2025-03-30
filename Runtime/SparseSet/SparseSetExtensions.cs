using System;
using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class SparseSetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static SparseSet CloneSparse(this SparseSet sparseSet)
		{
			var clone = new SparseSet();
			sparseSet.CopySparseTo(clone);
			return clone;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void CopySparseTo(this SparseSet source, SparseSet destination)
		{
			destination.EnsurePackedAt(source.Count - 1);
			destination.EnsureSparseAt(source.UsedIds - 1);

			Array.Copy(source.Packed, destination.Packed, source.Count);
			Array.Copy(source.Sparse, destination.Sparse, source.UsedIds);

			if (source.UsedIds < destination.UsedIds)
			{
				Array.Fill(destination.Sparse, Constants.InvalidId, source.UsedIds, destination.UsedIds - source.UsedIds);
			}

			destination.CurrentState = source.CurrentState;
		}
	}
}

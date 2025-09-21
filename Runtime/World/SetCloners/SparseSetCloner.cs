using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public sealed class SparseSetCloner<T> : SetCloner
	{
		private readonly BitSet _bitSet;

		public SparseSetCloner(BitSet bitSet)
		{
			_bitSet = bitSet;
		}

		public override void CopyTo(Sets sets)
		{
			_bitSet.CopyBitSetTo(sets.Get<T>());
		}
	}
}

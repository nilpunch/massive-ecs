using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public struct BitSetView<T1, T2, T3, T4>
	{
		public DataBitSet<T1> Set1 { get; }
		public DataBitSet<T2> Set2 { get; }
		public DataBitSet<T3> Set3 { get; }
		public DataBitSet<T4> Set4 { get; }

		public delegate void Iterator(ref T1 a, ref T2 b, ref T3 c, ref T4 d);

		public BitSetView(DataBitSet<T1> set1, DataBitSet<T2> set2, DataBitSet<T3> set3, DataBitSet<T4> set4)
		{
			Set1 = set1;
			Set2 = set2;
			Set3 = set3;
			Set4 = set4;
		}

		public void For(Iterator iterator)
		{
			var minBitSet = BitSet.GetMinBitSet(Set1.BitSet, Set2.BitSet, Set3.BitSet, Set4.BitSet);

			var resultBitSet = BitSetPool.Rent();
			minBitSet.CopyTo(resultBitSet);

			resultBitSet.And(Set1.BitSet).And(Set2.BitSet).And(Set3.BitSet).And(Set4.BitSet);

			Set1.PushRemoveOnRemove(resultBitSet);
			Set2.PushRemoveOnRemove(resultBitSet);
			Set3.PushRemoveOnRemove(resultBitSet);
			Set4.PushRemoveOnRemove(resultBitSet);

			var minBits1Length = resultBitSet.Bits1.Length;

			for (var current1 = 0; current1 < minBits1Length; current1++)
			{
				var offset1 = current1 << 6;
				var iterated1 = 0;

				while (resultBitSet.Bits1[current1] != 0UL && iterated1 < 64)
				{
					var bits1Result = resultBitSet.Bits1[current1] >> iterated1;

					var skip1 = MathUtils.TZC(bits1Result);
					iterated1 += skip1;
					bits1Result >>= skip1;

					if (bits1Result == 0UL)
					{
						break;
					}

					var runLength1 = bits1Result == ulong.MaxValue ? 64 : MathUtils.TZC(~bits1Result);
					var runEnd1 = iterated1 + runLength1;
					for (; iterated1 < runEnd1; iterated1++)
					{
						var current0 = offset1 + iterated1;
						var dataOffset1 = Set1.Pages[current0].DataIndex & Set1.PageSizeMinusOne;
						var dataOffset2 = Set2.Pages[current0].DataIndex & Set2.PageSizeMinusOne;
						var dataOffset3 = Set3.Pages[current0].DataIndex & Set3.PageSizeMinusOne;
						var dataOffset4 = Set4.Pages[current0].DataIndex & Set4.PageSizeMinusOne;
						var dataPage1 = Set1.Data[Set1.Pages[current0].DataIndex >> Set1.PageSizePower];
						var dataPage2 = Set2.Data[Set2.Pages[current0].DataIndex >> Set2.PageSizePower];
						var dataPage3 = Set3.Data[Set3.Pages[current0].DataIndex >> Set3.PageSizePower];
						var dataPage4 = Set4.Data[Set4.Pages[current0].DataIndex >> Set4.PageSizePower];

						var iterated0 = 0;

						while (resultBitSet.Bits0[current0] != 0UL && iterated0 < 64)
						{
							var bits0Result = resultBitSet.Bits0[current0] >> iterated0;

							var skip0 = MathUtils.TZC(bits0Result);
							iterated0 += skip0;
							bits0Result >>= skip0;

							if (bits0Result == 0UL)
							{
								break;
							}

							var runLength0 = bits0Result == ulong.MaxValue ? 64 : MathUtils.TZC(~bits0Result);
							var runEnd0 = iterated0 + runLength0;
							for (; iterated0 < runEnd0; iterated0++)
							{
								iterator.Invoke(
									ref dataPage1[dataOffset1 + iterated0],
									ref dataPage2[dataOffset2 + iterated0],
									ref dataPage3[dataOffset3 + iterated0],
									ref dataPage4[dataOffset4 + iterated0]);
							}
						}
					}
				}
			}

			Set1.PopRemoveOnRemove();
			Set2.PopRemoveOnRemove();
			Set3.PopRemoveOnRemove();
			Set4.PopRemoveOnRemove();

			BitSetPool.Return(resultBitSet);
		}
	}
}

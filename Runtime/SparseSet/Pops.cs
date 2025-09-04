using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class Pops
	{
		public List<BitsBase> PopOnAdd { get; } = new List<BitsBase>();
		public List<BitsBase> PopOnRemove { get; } = new List<BitsBase>();

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Pops AddPopOnAdd(BitsBase bits)
		{
			PopOnAdd.Add(bits);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Pops AddPopOnRemove(BitsBase bits)
		{
			PopOnRemove.Add(bits);
			return this;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void PopAll()
		{
			foreach (var bitSet in PopOnAdd)
			{
				bitSet.PopRemoveOnAdd();
			}
			foreach (var bitSet in PopOnRemove)
			{
				bitSet.PopRemoveOnRemove();
			}
			PopOnAdd.Clear();
			PopOnRemove.Clear();
		}
	}
}

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class InvalidPackedIndexException : MassiveException
	{
		private InvalidPackedIndexException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfNotPacked(SparseSet set, int index)
		{
			if (!set.HasPacked(index))
			{
				throw new InvalidPackedIndexException($"The packed index:{index} is not present in set.");
			}
		}
	}
}

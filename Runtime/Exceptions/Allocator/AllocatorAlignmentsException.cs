using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class AllocatorAlignmentsException : MassiveException
	{
		private AllocatorAlignmentsException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static unsafe void ThrowIfNotEqualAlignemnt(byte* src, byte* dst)
		{
			if (((ulong)src & 7) != ((ulong)dst & 7))
			{
				throw new AllocatorAlignmentsException("Source and destination pointers have different alignment.");
			}
		}
	}
}

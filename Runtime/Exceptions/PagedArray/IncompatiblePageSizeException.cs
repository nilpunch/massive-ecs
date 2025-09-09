using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Massive
{
	public class IncompatiblePageSizeException : MassiveException
	{
		private IncompatiblePageSizeException(string message) : base(message)
		{
		}

		[Conditional(Condition)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void ThrowIfIncompatible(IDataSet a, IDataSet b)
		{
			if (a.PageSize != b.PageSize)
			{
				throw new IncompatiblePageSizeException("Paged arrays has different page sizes.");
			}
		}
	}
}

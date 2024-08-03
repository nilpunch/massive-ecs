using System.Runtime.CompilerServices;

namespace Massive
{
	public static class DataSetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this IDataSet<T> dataSet, int id, T data)
		{
			dataSet.Assign(id);
			dataSet.Get(id) = data;
		}
	}
}

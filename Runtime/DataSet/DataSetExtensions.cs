using System.Runtime.CompilerServices;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public static class DataSetExtensions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Assign<T>(this DataSet<T> dataSet, int id, T data)
		{
			if (id < 0)
			{
				return;
			}

			dataSet.Assign(id);
			dataSet.Get(id) = data;
		}
	}
}

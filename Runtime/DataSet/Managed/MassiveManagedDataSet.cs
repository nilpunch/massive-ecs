using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/> with managed data support.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveManagedDataSet<T> : MassiveDataSetBase<T> where T : IManaged<T>
	{
		public MassiveManagedDataSet(int capacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			int pageSize = Constants.DefaultPageSize)
			: base(capacity, framesCapacity, pageSize)
		{
		}

		protected override void CopyData(T[] source, T[] destination, int count)
		{
			for (int i = 0; i < count; i++)
			{
				source[i].CopyTo(ref destination[i]);
			}
		}
	}
}

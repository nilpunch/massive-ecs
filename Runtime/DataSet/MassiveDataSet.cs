using System;
using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.MassiveSparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public class MassiveDataSet<T> : MassiveDataSetBase<T>
	{
		public MassiveDataSet(int setCapacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity, int pageSize = Constants.DefaultPageSize)
			: base(setCapacity, framesCapacity, pageSize)
		{
		}

		protected override void CopyData(T[] source, T[] destination, int count)
		{
			Array.Copy(source, destination, count);
		}
	}
}

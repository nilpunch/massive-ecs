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
		public MassiveDataSet(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
			: base(dataCapacity, framesCapacity)
		{
		}

		protected override void CopyData(T[] source, T[] destination, int count)
		{
			Array.Copy(source, destination, count);
		}
	}
}

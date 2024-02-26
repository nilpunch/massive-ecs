using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class DataSet<T> : DataSetBase<T, SparseSet> where T : unmanaged
	{
		public DataSet(int dataCapacity = Constants.DataCapacity) : base(new SparseSet(dataCapacity))
		{
		}
	}
}
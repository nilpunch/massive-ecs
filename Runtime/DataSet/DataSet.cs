namespace Massive
{
	/// <summary>
	/// Data extension for <see cref="Massive.SparseSet"/>.
	/// </summary>
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.NullChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.ArrayBoundsChecks, false)]
	[Unity.IL2CPP.CompilerServices.Il2CppSetOption(Unity.IL2CPP.CompilerServices.Option.DivideByZeroChecks, false)]
	public class DataSet<T> : DataSetBase<T, SparseSet> where T : unmanaged
	{
		public DataSet(int dataCapacity = Constants.DataCapacity) : base(new SparseSet(dataCapacity))
		{
		}
	}
}
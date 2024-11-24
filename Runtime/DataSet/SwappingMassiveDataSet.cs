using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	/// <summary>
	/// Rollback extension for <see cref="Massive.DataSet{T}"/>.
	/// Swaps data when elements are moved.
	/// </summary>
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public class SwappingMassiveDataSet<T> : MassiveDataSet<T>
	{
		public override void MoveDataAt(int source, int destination)
		{
			Data.Swap(source, destination);
		}
	}
}

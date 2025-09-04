using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public sealed class DataSetCloner<T> : SetCloner
	{
		private readonly DataSet<T> _dataSet;

		public DataSetCloner(DataSet<T> dataSet)
		{
			_dataSet = dataSet;
		}

		public override void CopyTo(BitSets bitSets)
		{
			_dataSet.CopyTo((DataSet<T>)bitSets.Get<T>());
		}
	}
}

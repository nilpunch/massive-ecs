using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public sealed class DataSetCloner<T> : SetCloner
	{
		private readonly DataSet<T> _dataSet;

		public DataSetCloner(DataSet<T> dataSet)
		{
			_dataSet = dataSet;
		}

		public override void CopyTo(SetRegistry setRegistry)
		{
			_dataSet.CopyTo((DataSet<T>)setRegistry.Get<T>());
		}
	}
}
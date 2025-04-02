using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public sealed class CopyingDataSetCloner<T> : SetCloner where T : ICopyable<T>
	{
		private readonly DataSet<T> _dataSet;

		public CopyingDataSetCloner(DataSet<T> dataSet)
		{
			_dataSet = dataSet;
		}

		public override void CopyTo(SetRegistry setRegistry)
		{
			_dataSet.CopyToCopyable((DataSet<T>)setRegistry.Get<T>());
		}
	}
}
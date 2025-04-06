using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	[Il2CppSetOption(Option.DivideByZeroChecks, false)]
	public sealed class CopyingDataSetClonerByIdentifier<T> : SetCloner where T : ICopyable<T>
	{
		private readonly CopyingDataSet<T> _dataSet;
		private readonly string _setId;

		public CopyingDataSetClonerByIdentifier(CopyingDataSet<T> dataSet, string setId)
		{
			_dataSet = dataSet;
			_setId = setId;
		}

		public override void CopyTo(SetRegistry setRegistry)
		{
			var destination = setRegistry.GetExisting(_setId);

			if (destination == null)
			{
				var clone = _dataSet.CloneCopyable();
				setRegistry.Insert(_setId, clone, new CopyingDataSetClonerByIdentifier<T>(clone, _setId));
			}
			else
			{
				_dataSet.CopyToCopyable((DataSet<T>)destination);
			}
		}
	}
}

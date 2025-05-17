using Unity.IL2CPP.CompilerServices;

namespace Massive
{
	[Il2CppSetOption(Option.NullChecks, false)]
	[Il2CppSetOption(Option.ArrayBoundsChecks, false)]
	public sealed class DataSetClonerByIdentifier<T> : SetCloner
	{
		private readonly DataSet<T> _dataSet;
		private readonly string _setId;

		public DataSetClonerByIdentifier(DataSet<T> dataSet, string setId)
		{
			_dataSet = dataSet;
			_setId = setId;
		}

		public override void CopyTo(Sets sets)
		{
			var destination = sets.GetExisting(_setId);

			if (destination == null)
			{
				var clone = _dataSet.Clone();
				sets.Insert(_setId, clone, new DataSetClonerByIdentifier<T>(clone, _setId));
			}
			else
			{
				_dataSet.CopyTo((DataSet<T>)destination);
			}
		}
	}
}

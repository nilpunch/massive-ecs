namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly int _dataCapacity;
		private readonly bool _storeEmptyTypesAsDataSets;

		public NormalSetFactory(int dataCapacity = Constants.DataCapacity, bool storeEmptyTypesAsDataSets = false)
		{
			_dataCapacity = dataCapacity;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
		}

		public ISet CreateAppropriateSet<T>()
		{
			if (TypeInfo<T>.HasNoFields && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet();
			}

			return CreateDataSet<T>();
		}

		private ISet CreateSparseSet()
		{
			return new SparseSet(_dataCapacity);
		}

		private ISet CreateDataSet<T>()
		{
			return new DataSet<T>(_dataCapacity);
		}
	}
}

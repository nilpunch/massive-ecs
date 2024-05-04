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

		public Entities CreateEntities()
		{
			return new Entities(_dataCapacity);
		}

		public ISet CreateAppropriateSet<T>()
		{
			if (TypeInfo<T>.HasNoFields && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet();
			}

			return CreateDataSet<T>();
		}

		public ISet CreateSparseSet()
		{
			return new SparseSet(_dataCapacity);
		}

		public ISet CreateDataSet<T>()
		{
			return new DataSet<T>(_dataCapacity);
		}
	}
}

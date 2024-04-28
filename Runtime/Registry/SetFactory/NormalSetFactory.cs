namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly int _dataCapacity;
		private readonly bool _storeTagsAsComponents;

		public NormalSetFactory(int dataCapacity = Constants.DataCapacity, bool storeTagsAsComponents = false)
		{
			_dataCapacity = dataCapacity;
			_storeTagsAsComponents = storeTagsAsComponents;
		}

		public Entities CreateEntities()
		{
			return new Entities(_dataCapacity);
		}

		public ISet CreateAppropriateSet<T>()
		{
			if (TypeInfo<T>.HasNoFields && !_storeTagsAsComponents)
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

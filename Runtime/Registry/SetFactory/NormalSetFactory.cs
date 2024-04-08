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

		public Entities CreateIdentifiers()
		{
			return new Entities(_dataCapacity);
		}

		public ISet CreateAppropriateSet<T>() where T : struct
		{
			if (Type<T>.HasNoFields && !_storeTagsAsComponents)
			{
				return CreateSparseSet();
			}

			return CreateDataSet<T>();
		}

		public ISet CreateSparseSet()
		{
			return new SparseSet(_dataCapacity);
		}

		public ISet CreateDataSet<T>() where T : struct
		{
			if (ManagedUtils.IsManaged<T>())
			{
				return ManagedUtils.CreateManagedDataSet<T>(_dataCapacity);
			}
			else
			{
				return new DataSet<T>(_dataCapacity);
			}
		}
	}
}
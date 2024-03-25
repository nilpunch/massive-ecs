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

		public ISet CreateSet()
		{
			return new SparseSet(_dataCapacity);
		}

		public ISet CreateSet<T>() where T : struct
		{
			if (Type<T>.HasNoFields && !_storeTagsAsComponents)
			{
				return new SparseSet(_dataCapacity);
			}

			if (ManagedUtils.IsManaged<T>())
			{
				return ManagedUtils.CreateManagedDataSet<T>(_dataCapacity);
			}
			else
			{
				return new DataSet<T>(_dataCapacity);
			}
		}

		public Identifiers CreateIdentifiers()
		{
			return new Identifiers(_dataCapacity);
		}
	}
}
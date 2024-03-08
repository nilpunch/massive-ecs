namespace Massive
{
	public class NormalSetFactory : ISetFactory
	{
		private readonly int _dataCapacity;

		public NormalSetFactory(int dataCapacity = Constants.DataCapacity)
		{
			_dataCapacity = dataCapacity;
		}

		public ISet CreateSet()
		{
			return new SparseSet(_dataCapacity);
		}

		public ISet CreateDataSet<T>() where T : struct
		{
			if (ManagedUtils.IsManaged<T>())
			{
				return ManagedUtils.CreateDataSet<T>(_dataCapacity);
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
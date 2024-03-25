namespace Massive
{
	/// <summary>
	/// Factory for data structures with rollbacks.
	/// </summary>
	/// <remarks>
	/// Created structures have first empty frame saved so that you can rollback to it.
	/// </remarks>
	public class MassiveSetFactory : ISetFactory
	{
		private readonly int _dataCapacity;
		private readonly int _framesCapacity;
		private readonly bool _storeTagsAsComponents;

		public MassiveSetFactory(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity, bool storeTagsAsComponents = false)
		{
			_dataCapacity = dataCapacity;
			_framesCapacity = framesCapacity;
			_storeTagsAsComponents = storeTagsAsComponents;
		}

		public Identifiers CreateIdentifiers()
		{
			var massiveIdentifiers = new MassiveIdentifiers(_dataCapacity, _framesCapacity);
			massiveIdentifiers.SaveFrame();
			return massiveIdentifiers;
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
			var massiveSparseSet = new MassiveSparseSet(_dataCapacity, _framesCapacity);
			massiveSparseSet.SaveFrame();
			return massiveSparseSet;
		}

		public ISet CreateDataSet<T>() where T : struct
		{
			if (ManagedUtils.IsManaged<T>())
			{
				var massiveManagedDataSet = ManagedUtils.CreateMassiveManagedDataSet<T>(_dataCapacity, _framesCapacity);
				((IMassive)massiveManagedDataSet).SaveFrame();
				return massiveManagedDataSet;
			}
			else
			{
				var massiveDataSet = new MassiveDataSet<T>(_dataCapacity, _framesCapacity);
				massiveDataSet.SaveFrame();
				return massiveDataSet;
			}
		}
	}
}
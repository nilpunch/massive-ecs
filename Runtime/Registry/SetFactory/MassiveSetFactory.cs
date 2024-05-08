namespace Massive
{
	/// <summary>
	/// Factory for data structures with rollbacks.
	/// </summary>
	/// <remarks>
	/// Created massives have first frame saved so that you can rollback to it.
	/// </remarks>
	public class MassiveSetFactory : ISetFactory
	{
		private readonly int _dataCapacity;
		private readonly int _framesCapacity;
		private readonly bool _storeEmptyTypesAsDataSets;

		public MassiveSetFactory(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity, bool storeEmptyTypesAsDataSets = false)
		{
			_dataCapacity = dataCapacity;
			_framesCapacity = framesCapacity;
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
			var massiveSparseSet = new MassiveSparseSet(_dataCapacity, _framesCapacity);
			massiveSparseSet.SaveFrame();
			return massiveSparseSet;
		}

		private ISet CreateDataSet<T>()
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

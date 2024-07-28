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
		private readonly int _setCapacity;
		private readonly int _framesCapacity;
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;

		public MassiveSetFactory(int setCapacity = Constants.DefaultSetCapacity, int framesCapacity = Constants.DefaultFramesCapacity,
			bool storeEmptyTypesAsDataSets = false, int pageSize = Constants.DefaultPageSize)
		{
			_setCapacity = setCapacity;
			_framesCapacity = framesCapacity;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
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
			var massiveSparseSet = new MassiveSparseSet(_setCapacity, _framesCapacity);
			massiveSparseSet.SaveFrame();
			return massiveSparseSet;
		}

		private ISet CreateDataSet<T>()
		{
			var massiveDataSet = ManagedUtils.IsManaged<T>()
				? ManagedUtils.CreateMassiveManagedDataSet<T>(_setCapacity, _framesCapacity, _pageSize)
				: new MassiveDataSet<T>(_setCapacity, _framesCapacity, _pageSize);

			((IMassive)massiveDataSet).SaveFrame();
			return massiveDataSet;
		}
	}
}

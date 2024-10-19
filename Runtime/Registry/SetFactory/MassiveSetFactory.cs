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
		private readonly bool _fullStability;

		public MassiveSetFactory(int setCapacity = Constants.DefaultCapacity, int framesCapacity = Constants.DefaultFramesCapacity, bool storeEmptyTypesAsDataSets = false,
			int pageSize = Constants.DefaultPageSize, bool fullStability = false)
		{
			_setCapacity = setCapacity;
			_framesCapacity = framesCapacity;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
			_fullStability = fullStability;
		}

		public SparseSet CreateAppropriateSet<T>()
		{
			if (TypeInfo<T>.HasNoFields && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet<T>();
			}

			return CreateDataSet<T>();
		}

		private SparseSet CreateSparseSet<T>()
		{
			var massiveSparseSet = new MassiveSparseSet(_setCapacity, _framesCapacity, GetPackingModeFor<T>());
			massiveSparseSet.SaveFrame();
			return massiveSparseSet;
		}

		private SparseSet CreateDataSet<T>()
		{
			var massiveDataSet = ManagedUtils.IsManaged<T>()
				? ManagedUtils.CreateMassiveManagedDataSet<T>(_setCapacity, _framesCapacity, _pageSize, GetPackingModeFor<T>())
				: new MassiveDataSet<T>(_setCapacity, _framesCapacity, _pageSize, GetPackingModeFor<T>());
			((IMassive)massiveDataSet).SaveFrame();
			return massiveDataSet;
		}

		private PackingMode GetPackingModeFor<T>()
		{
			return _fullStability || IStable.IsImplementedFor<T>() ? PackingMode.WithHoles : PackingMode.Continuous;
		}
	}
}

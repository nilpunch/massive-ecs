using System;

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
		private readonly int _framesCapacity;
		private readonly bool _storeEmptyTypesAsDataSets;
		private readonly int _pageSize;
		private readonly bool _fullStability;

		public MassiveSetFactory(MassiveWorldConfig worldConfig)
			: this(worldConfig.FramesCapacity, worldConfig.StoreEmptyTypesAsDataSets,
				worldConfig.PageSize, worldConfig.FullStability)
		{
		}

		public MassiveSetFactory(int framesCapacity = Constants.DefaultFramesCapacity, bool storeEmptyTypesAsDataSets = false,
			int pageSize = Constants.DefaultPageSize, bool fullStability = false)
		{
			_framesCapacity = framesCapacity;
			_storeEmptyTypesAsDataSets = storeEmptyTypesAsDataSets;
			_pageSize = pageSize;
			_fullStability = fullStability;
		}

		public SparseSet CreateAppropriateSet<T>()
		{
			var type = typeof(T);

			if (type.IsValueType && ReflectionUtils.HasNoFields(typeof(T)) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet(GetPackingFor(typeof(T)));
			}

			return CreateDataSet<T>();
		}

		private SparseSet CreateSparseSet(Packing packing)
		{
			var massiveSparseSet = new MassiveSparseSet(_framesCapacity, packing);
			massiveSparseSet.SaveFrame();
			return massiveSparseSet;
		}

		private SparseSet CreateDataSet<T>()
		{
			SparseSet massiveDataSet;
			if (CopyableUtils.IsImplementedFor(typeof(T)))
			{
				massiveDataSet = CopyableUtils.CreateMassiveCopyableDataSet<T>(_framesCapacity, _pageSize, GetPackingFor(typeof(T)));
			}
			else if (typeof(T).IsManaged())
			{
				massiveDataSet = new MassiveSwappingDataSet<T>(_framesCapacity, _pageSize, GetPackingFor(typeof(T)));
			}
			else
			{
				massiveDataSet = new MassiveDataSet<T>(_framesCapacity, _pageSize, GetPackingFor(typeof(T)));
			}
			((IMassive)massiveDataSet).SaveFrame();
			return massiveDataSet;
		}

		private Packing GetPackingFor(Type type)
		{
			return _fullStability || IStable.IsImplementedFor(type) ? Packing.WithHoles : Packing.Continuous;
		}
	}
}

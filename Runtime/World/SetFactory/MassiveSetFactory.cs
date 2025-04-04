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

		public ISetFactory.Output CreateAppropriateSet<T>()
		{
			var type = typeof(T);

			if (type.IsValueType && ReflectionUtils.HasNoFields(typeof(T)) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet<T>();
			}

			return CreateDataSet<T>();
		}

		private ISetFactory.Output CreateSparseSet<T>()
		{
			var sparseSet = new MassiveSparseSet(_framesCapacity, GetPackingFor(typeof(T)));
			var cloner = new SparseSetCloner<T>(sparseSet);
			sparseSet.SaveFrame();
			return new ISetFactory.Output(sparseSet, cloner);
		}

		private ISetFactory.Output CreateDataSet<T>()
		{
			DataSet<T> dataSet;
			SetCloner cloner;
			if (CopyableUtils.IsImplementedFor(typeof(T)))
			{
				dataSet = CopyableUtils.CreateMassiveCopyableDataSet<T>(_framesCapacity, _pageSize, GetPackingFor(typeof(T)));
				cloner = CopyableUtils.CreateCopyingDataSetCloner(dataSet);
			}
			else if (typeof(T).IsManaged())
			{
				dataSet = new MassiveSwappingDataSet<T>(_framesCapacity, _pageSize, GetPackingFor(typeof(T)));
				cloner = new DataSetCloner<T>(dataSet);
			}
			else
			{
				dataSet = new MassiveDataSet<T>(_framesCapacity, _pageSize, GetPackingFor(typeof(T)));
				cloner = new DataSetCloner<T>(dataSet);
			}
			((IMassive)dataSet).SaveFrame();
			return new ISetFactory.Output(dataSet, cloner);
		}

		private Packing GetPackingFor(Type type)
		{
			return _fullStability || IStable.IsImplementedFor(type) ? Packing.WithHoles : Packing.Continuous;
		}
	}
}

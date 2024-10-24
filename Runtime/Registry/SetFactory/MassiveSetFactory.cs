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
			if (TypeInfo.HasNoFields(typeof(T)) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet(GetPackingModeFor(typeof(T)));
			}

			return CreateDataSet<T>();
		}

		public SparseSet CreateAppropriateSet(Type type)
		{
			if (TypeInfo.HasNoFields(type) && !_storeEmptyTypesAsDataSets)
			{
				return CreateSparseSet(GetPackingModeFor(type));
			}

			var args = new object[] { _setCapacity, _framesCapacity, _pageSize, GetPackingModeFor(type) };
			var massiveDataSet = ManagedUtils.IsManaged(type)
				? ReflectionHelpers.CreateGeneric(typeof(MassiveManagedDataSet<>), type, args)
				: ReflectionHelpers.CreateGeneric(typeof(MassiveDataSet<>), type, args);
			((IMassive)massiveDataSet).SaveFrame();
			return (SparseSet)massiveDataSet;
		}

		private SparseSet CreateSparseSet(PackingMode packingMode)
		{
			var massiveSparseSet = new MassiveSparseSet(_setCapacity, _framesCapacity, packingMode);
			massiveSparseSet.SaveFrame();
			return massiveSparseSet;
		}

		private SparseSet CreateDataSet<T>()
		{
			var massiveDataSet = ManagedUtils.IsManaged<T>()
				? ManagedUtils.CreateMassiveManagedDataSet<T>(_setCapacity, _framesCapacity, _pageSize, GetPackingModeFor(typeof(T)))
				: new MassiveDataSet<T>(_setCapacity, _framesCapacity, _pageSize, GetPackingModeFor(typeof(T)));
			((IMassive)massiveDataSet).SaveFrame();
			return massiveDataSet;
		}

		private PackingMode GetPackingModeFor(Type type)
		{
			return _fullStability || IStable.IsImplementedFor(type) ? PackingMode.WithHoles : PackingMode.Continuous;
		}
	}
}

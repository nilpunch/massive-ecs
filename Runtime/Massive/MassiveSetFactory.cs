﻿namespace Massive
{
	public class MassiveSetFactory : ISetFactory
	{
		private readonly int _framesCapacity;
		private readonly int _dataCapacity;

		public MassiveSetFactory(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
		{
			_framesCapacity = framesCapacity;
			_dataCapacity = dataCapacity;
		}

		public ISet CreateSet()
		{
			var massiveSparseSet = new MassiveSparseSet(_framesCapacity, _dataCapacity);

			// Save first empty frame to ensure we can rollback to it
			massiveSparseSet.SaveFrame();

			return massiveSparseSet;
		}

		public ISet CreateDataSet<T>() where T : struct
		{
			if (ComponentMeta<T>.IsManaged)
			{
				return CreateMassiveManagedDataSet<T>();
			}
			else
			{
				return CreateMassiveNormalDataSet<T>();
			}
		}

		public Identifiers CreateIdentifiers()
		{
			var massiveIdentifiers = new MassiveIdentifiers(_framesCapacity, _dataCapacity);

			// Save first empty frame to ensure we can rollback to it
			massiveIdentifiers.SaveFrame();

			return massiveIdentifiers;
		}

		private ISet CreateMassiveNormalDataSet<T>() where T : struct
		{
			var massiveDataSet = new MassiveDataSet<T>(_framesCapacity, _dataCapacity);

			// Save first empty frame to ensure we can rollback to it
			massiveDataSet.SaveFrame();

			return massiveDataSet;
		}

		private ISet CreateMassiveManagedDataSet<T>() where T : struct
		{
			var massiveManagedDataSet = Managed.CreateMassiveDataSet<T>(_framesCapacity, _dataCapacity);

			// Save first empty frame to ensure we can rollback to it
			((IMassive)massiveManagedDataSet).SaveFrame();

			return massiveManagedDataSet;
		}
	}
}
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
			massiveSparseSet.SaveFrame();
			return massiveSparseSet;
		}

		public ISet CreateDataSet<T>() where T : struct
		{
			if (ManagedUtils.IsManaged<T>())
			{
				var massiveManagedDataSet = ManagedUtils.CreateMassiveManagedDataSet<T>(_framesCapacity, _dataCapacity);
				((IMassive)massiveManagedDataSet).SaveFrame();
				return massiveManagedDataSet;
			}
			else
			{
				var massiveDataSet = new MassiveDataSet<T>(_framesCapacity, _dataCapacity);
				massiveDataSet.SaveFrame();
				return massiveDataSet;
			}
		}

		public Identifiers CreateIdentifiers()
		{
			var massiveIdentifiers = new MassiveIdentifiers(dataCapacity: _dataCapacity, framesCapacity: _framesCapacity);
			massiveIdentifiers.SaveFrame();
			return massiveIdentifiers;
		}
	}
}
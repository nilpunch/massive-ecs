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

		public MassiveSetFactory(int dataCapacity = Constants.DataCapacity, int framesCapacity = Constants.FramesCapacity)
		{
			_framesCapacity = framesCapacity;
			_dataCapacity = dataCapacity;
		}

		public ISet CreateSet()
		{
			var massiveSparseSet = new MassiveSparseSet(_dataCapacity, _framesCapacity);
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
				var massiveDataSet = new MassiveDataSet<T>(_dataCapacity, _framesCapacity);
				massiveDataSet.SaveFrame();
				return massiveDataSet;
			}
		}

		public Identifiers CreateIdentifiers()
		{
			var massiveIdentifiers = new MassiveIdentifiers(_dataCapacity, _framesCapacity);
			massiveIdentifiers.SaveFrame();
			return massiveIdentifiers;
		}
	}
}
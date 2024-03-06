namespace Massive.ECS
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
			var massiveDataSet = new MassiveDataSet<T>(_framesCapacity, _dataCapacity);

			// Save first empty frame to ensure we can rollback to it
			massiveDataSet.SaveFrame();

			return massiveDataSet;
		}

		public Identifiers CreateIdentifiers()
		{
			var massiveIdentifiers = new MassiveIdentifiers(_framesCapacity, _dataCapacity);

			// Save first empty frame to ensure we can rollback to it
			massiveIdentifiers.SaveFrame();

			return massiveIdentifiers;
		}
	}
}
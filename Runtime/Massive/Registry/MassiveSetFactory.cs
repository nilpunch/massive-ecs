namespace Massive.ECS
{
	public class MassiveSetFactory : ISetFactory<IMassiveSet>
	{
		private readonly int _framesCapacity;
		private readonly int _dataCapacity;

		public MassiveSetFactory(int framesCapacity = Constants.FramesCapacity, int dataCapacity = Constants.DataCapacity)
		{
			_framesCapacity = framesCapacity;
			_dataCapacity = dataCapacity;
		}

		public IMassiveSet CreateSet()
		{
			var massiveSparseSet = new MassiveSparseSet(_framesCapacity, _dataCapacity);

			// Save first empty frame to ensure we can rollback to it
			massiveSparseSet.SaveFrame();

			return massiveSparseSet;
		}

		public IMassiveSet CreateDataSet<T>() where T : unmanaged
		{
			var massiveDataSet = new MassiveDataSet<T>(_framesCapacity, _dataCapacity);

			// Save first empty frame to ensure we can rollback to it
			massiveDataSet.SaveFrame();

			return massiveDataSet;
		}
	}
}
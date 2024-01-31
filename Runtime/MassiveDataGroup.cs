namespace Massive
{
	public class MassiveDataGroup : IMassiveData
	{
		private readonly IMassiveData[] _worldStates;

		public MassiveDataGroup(IMassiveData[] worldStates)
		{
			_worldStates = worldStates;
		}

		public void SaveFrame()
		{
			foreach (var worldState in _worldStates)
			{
				worldState.SaveFrame();
			}
		}

		public void Rollback(int frames)
		{
			foreach (var worldState in _worldStates)
			{
				worldState.Rollback(frames);
			}
		}
	}
}
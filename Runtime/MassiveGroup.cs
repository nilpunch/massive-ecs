namespace MassiveData
{
	public class MassiveGroup : IMassive
	{
		private readonly IMassive[] _worldStates;

		public MassiveGroup(IMassive[] worldStates)
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
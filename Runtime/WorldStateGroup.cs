namespace Massive
{
	public class WorldStateGroup : IWorldState
	{
		private readonly IWorldState[] _worldStates;

		public WorldStateGroup(IWorldState[] worldStates)
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
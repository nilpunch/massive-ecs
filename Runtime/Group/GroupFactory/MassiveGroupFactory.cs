namespace Massive
{
	public class MassiveGroupFactory : IGroupFactory
	{
		private readonly int _framesCapacity;

		public MassiveGroupFactory(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_framesCapacity = framesCapacity;
		}

		public Group CreateGroup(SparseSet[] include = null, SparseSet[] exclude = null, Entities entities = null)
		{
			var massiveGroup = new MassiveGroup(include, exclude, _framesCapacity, entities);
			massiveGroup.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveGroup;
		}
	}
}

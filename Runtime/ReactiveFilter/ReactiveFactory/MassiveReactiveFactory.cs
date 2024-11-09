namespace Massive
{
	public class MassiveReactiveFactory : IReactiveFactory
	{
		private readonly int _framesCapacity;

		public MassiveReactiveFactory(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_framesCapacity = framesCapacity;
		}

		public ReactiveFilter CreateReactiveFilter(SparseSet[] include = null, SparseSet[] exclude = null, Entities entities = null)
		{
			var massiveReactiveFilter = new MassiveReactiveFilter(include, exclude, _framesCapacity, entities);
			massiveReactiveFilter.SaveFrame(); // Save first empty frame so we can rollback to it
			return massiveReactiveFilter;
		}
	}
}

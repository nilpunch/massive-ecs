namespace Massive
{
	public interface IQueryable
	{
		public Query.Context Context { get; }

		public World World => Context.World;

		public Filter Filter => Context.Filter;

		BitsEnumerator GetEnumerator();
	}
}

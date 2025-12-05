namespace Massive
{
	public interface IQueryable
	{
		Query Query { get; }

		World World => Query.World;

		Filter Filter => Query.Filter;

		EntityEnumerable Entities { get; }

		IdsEnumerator GetEnumerator();
	}
}

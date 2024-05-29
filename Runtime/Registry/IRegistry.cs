namespace Massive
{
	public interface IRegistry
	{
		SetRegistry SetRegistry { get; }
		FilterRegistry FilterRegistry { get; }
		GroupRegistry GroupRegistry { get; }
		IEntities Entities { get; }
	}
}

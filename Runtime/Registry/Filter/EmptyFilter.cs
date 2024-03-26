namespace Massive
{
	public class EmptyFilter : IFilter
	{
		public bool Contains(int id)
		{
			return true;
		}
	}
}
namespace Massive
{
	public delegate void EntityAction(int id);

	public delegate void EntityActionExtra<TExtra>(int id, TExtra extra);
}

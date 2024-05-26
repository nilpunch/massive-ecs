namespace Massive
{
	public delegate void EntityAction(int id);

	public delegate void EntityActionRef<T>(int id, ref T a);

	public delegate void EntityActionRef<T1, T2>(int id, ref T1 a, ref T2 b);

	public delegate void EntityActionRef<T1, T2, T3>(int id, ref T1 a, ref T2 b, ref T3 c);

	public delegate void EntityActionExtra<TExtra>(int id, TExtra extra);

	public delegate void EntityActionRefExtra<T, TExtra>(int id, ref T a, TExtra extra);

	public delegate void EntityActionRefExtra<T1, T2, TExtra>(int id, ref T1 a, ref T2 b, TExtra extra);

	public delegate void EntityActionRefExtra<T1, T2, T3, TExtra>(int id, ref T1 a, ref T2 b, ref T3 c, TExtra extra);
}

namespace Massive
{
	public delegate void EntityAction(int entityId);

	public delegate void EntityActionRef<T>(int entityId, ref T a);

	public delegate void EntityActionRef<T1, T2>(int entityId, ref T1 a, ref T2 b);

	public delegate void EntityActionRef<T1, T2, T3>(int entityId, ref T1 a, ref T2 b, ref T3 c);

	public delegate void EntityActionExtra<in TExtra>(int entityId, TExtra extra);

	public delegate void EntityActionRefExtra<T, in TExtra>(int entityId, ref T a, TExtra extra);

	public delegate void EntityActionRefExtra<T1, T2, in TExtra>(int entityId, ref T1 a, ref T2 b, TExtra extra);

	public delegate void EntityActionRefExtra<T1, T2, T3, in TExtra>(int entityId, ref T1 a, ref T2 b, ref T3 c, TExtra extra);
}
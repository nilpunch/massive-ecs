namespace Massive
{
	public delegate void EntityAction(int entity);

	public delegate void EntityActionRef<T>(int entity, ref T a);

	public delegate void EntityActionRef<T1, T2>(int entity, ref T1 a, ref T2 b);

	public delegate void EntityActionRef<T1, T2, T3>(int entity, ref T1 a, ref T2 b, ref T3 c);

	public delegate void EntityActionExtra<in TExtra>(int entity, TExtra extra);

	public delegate void EntityActionRefExtra<T, in TExtra>(int entity, ref T a, TExtra extra);

	public delegate void EntityActionRefExtra<T1, T2, in TExtra>(int entity, ref T1 a, ref T2 b, TExtra extra);

	public delegate void EntityActionRefExtra<T1, T2, T3, in TExtra>(int entity, ref T1 a, ref T2 b, ref T3 c, TExtra extra);
}
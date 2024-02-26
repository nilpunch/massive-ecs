namespace Massive.ECS
{
	public delegate void EntityAction(Entity entity);

	public delegate void ActionRef<T>(ref T value);

	public delegate void EntityActionRef<T>(Entity entity, ref T value);

	public delegate void ActionRef<T1, T2>(ref T1 value1, ref T2 value2);

	public delegate void EntityActionRef<T1, T2>(Entity entity, ref T1 value1, ref T2 value2);
}
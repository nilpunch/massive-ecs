namespace Massive.ECS
{
	public class Registry : RegistryBase<ISet>
	{
		protected Registry(int dataCapacity = Constants.DataCapacity)
			: base(new NormalSetFactory(dataCapacity))
		{
		}
	}
}
namespace Massive
{
	public interface IView
	{
		Registry Registry { get; }

		void ForEach<TAction>(ref TAction action)
			where TAction : IEntityAction;
	}
}

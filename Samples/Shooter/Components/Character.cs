namespace Massive.Samples.Shooter
{
	public struct Character
	{
		public readonly int MaxHealth;

		public int Health;

		public Character(int maxHealth)
		{
			MaxHealth = maxHealth;
			Health = MaxHealth;
		}
	}
}

namespace Massive.Samples.Shooter
{
	public struct Character : IAuto<Character>
	{
		public readonly int MaxHealth;

		public int Health;

		public ListPointer<Entifier> Bullets;

		public Character(int maxHealth, ListPointer<Entifier> bullets)
		{
			MaxHealth = maxHealth;
			Bullets = bullets;
			Health = MaxHealth;
		}
	}
}

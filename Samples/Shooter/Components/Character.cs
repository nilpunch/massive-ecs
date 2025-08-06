namespace Massive.Samples.Shooter
{
	public struct Character
	{
		public readonly int MaxHealth;

		public int Health;

		public ListHandle<Entifier> Bullets;

		public Character(int maxHealth, ListHandle<Entifier> bullets)
		{
			MaxHealth = maxHealth;
			Bullets = bullets;
			Health = MaxHealth;
		}
	}
}

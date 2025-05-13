namespace Massive.Samples.Shooter
{
	public struct Character
	{
		public readonly int MaxHealth;

		public int Health;

		public ListHandle<Entity> Bullets;

		public Character(int maxHealth, ListHandle<Entity> bullets)
		{
			MaxHealth = maxHealth;
			Bullets = bullets;
			Health = MaxHealth;
		}
	}
}

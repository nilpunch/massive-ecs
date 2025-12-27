namespace Massive.Samples.Shooter
{
	public class DestroyDeadCharactersBulletsSystem : WorldSystem, IUpdate
	{
		public void Update(float deltaTime)
		{
			World.Include<Dead>().ForEach(World,
				static (ref Character character, World world) =>
				{
					var characterBullets = character.Bullets.In(world);

					foreach (var bullet in characterBullets)
					{
						bullet.In(world).Add<Dead>();
					}

					characterBullets.Clear();
				});
		}
	}
}

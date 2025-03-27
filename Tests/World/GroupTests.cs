using System;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class GroupTests
	{
		[Test]
		public void IncludeFilter()
		{
			var world = new World();

			var group = SetUpWithIntAndChar(world);

			NUnit.Framework.Assert.AreEqual(0, group.Count);

			var entity1 = world.CreateEntity('1').Id;
			var entity2 = world.CreateEntity('2').Id;

			world.Assign(entity2, 42);

			NUnit.Framework.Assert.AreEqual(1, group.Count);

			world.Assign<int>(entity1);

			NUnit.Framework.Assert.AreEqual(2, group.Count);

			world.Unassign<int>(entity1);

			NUnit.Framework.Assert.AreEqual(1, group.Count);

			foreach (var id in group)
			{
				NUnit.Framework.Assert.AreEqual(world.Get<int>(id), 42);
				NUnit.Framework.Assert.AreEqual(world.Get<char>(id), '2');
			}

			var enumerator = group.GetEnumerator();
			enumerator.MoveNext();
			NUnit.Framework.Assert.AreEqual(enumerator.Current, entity2);
			enumerator.Dispose();

			world.Unassign<char>(entity1);
			world.Unassign<char>(entity2);

			NUnit.Framework.Assert.AreEqual(0, group.Count);
		}

		[Test]
		public void ExcludeFilter()
		{
			var world = new World();

			var entity1 = world.CreateEntity(1).Id;

			var entity2 = world.CreateEntity(2).Id;
			world.Assign<char>(entity2);

			var group = SetUpWithIntAndWithoutChar(world);

			var entity3 = world.CreateEntity(3).Id;

			var entity4 = world.CreateEntity(4).Id;
			world.Assign<char>(entity4);

			foreach (var entity in group)
			{
				NUnit.Framework.Assert.True(entity == entity1 || entity == entity3);

				if (entity == entity1)
				{
					NUnit.Framework.Assert.AreEqual(1, world.Get<int>(entity1));
				}
				else if (entity == entity3)
				{
					NUnit.Framework.Assert.AreEqual(3, world.Get<int>(entity3));
				}
			}

			world.Assign<char>(entity1);
			world.Assign<char>(entity3);

			NUnit.Framework.Assert.AreEqual(0, group.Count);

			world.Unassign<char>(entity2);
			world.Unassign<char>(entity4);

			foreach (var entity in group)
			{
				NUnit.Framework.Assert.True(entity == entity2 || entity == entity4);

				if (entity == entity2)
				{
					NUnit.Framework.Assert.AreEqual(2, world.Get<int>(entity2));
				}
				else if (entity == entity4)
				{
					NUnit.Framework.Assert.AreEqual(4, world.Get<int>(entity4));
				}
			}
		}

		[Test]
		public void StableLateInitialization()
		{
			var world = new World();

			for (int i = 0; i < 30; i++)
			{
				var entity = world.CreateEntity();

				if (i % 2 != 0)
				{
					world.Assign<int>(entity);
				}

				if (i % 3 != 0)
				{
					world.Assign<char>(entity);
				}
			}

			var group = SetUpWithIntAndWithoutChar(world);

			NUnit.Framework.Assert.AreEqual(5, group.Count);
		}

		private Group SetUpWithIntAndChar(World world)
		{
			return world.Group<Include<int, char>>();
		}

		private Group SetUpWithIntAndWithoutChar(World world)
		{
			return world.Group<Include<int>, Exclude<char>>();

		}
	}
}

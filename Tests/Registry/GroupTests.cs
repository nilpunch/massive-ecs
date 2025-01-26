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
			var registry = new Registry();

			var group = SetUpWithIntAndChar(registry);

			NUnit.Framework.Assert.AreEqual(0, group.Count);

			var entity1 = registry.CreateEntity('1').Id;
			var entity2 = registry.CreateEntity('2').Id;

			registry.Assign(entity2, 42);

			NUnit.Framework.Assert.AreEqual(1, group.Count);

			registry.Assign<int>(entity1);

			NUnit.Framework.Assert.AreEqual(2, group.Count);

			registry.Unassign<int>(entity1);

			NUnit.Framework.Assert.AreEqual(1, group.Count);

			foreach (var id in group)
			{
				NUnit.Framework.Assert.AreEqual(registry.Get<int>(id), 42);
				NUnit.Framework.Assert.AreEqual(registry.Get<char>(id), '2');
			}

			var enumerator = group.GetEnumerator();
			enumerator.MoveNext();
			NUnit.Framework.Assert.AreEqual(enumerator.Current, entity2);
			enumerator.Dispose();

			registry.Unassign<char>(entity1);
			registry.Unassign<char>(entity2);

			NUnit.Framework.Assert.AreEqual(0, group.Count);
		}

		[Test]
		public void ExcludeFilter()
		{
			var registry = new Registry();

			var entity1 = registry.CreateEntity(1).Id;

			var entity2 = registry.CreateEntity(2).Id;
			registry.Assign<char>(entity2);

			var group = SetUpWithIntAndWithoutChar(registry);

			var entity3 = registry.CreateEntity(3).Id;

			var entity4 = registry.CreateEntity(4).Id;
			registry.Assign<char>(entity4);

			foreach (var entity in group)
			{
				NUnit.Framework.Assert.True(entity == entity1 || entity == entity3);

				if (entity == entity1)
				{
					NUnit.Framework.Assert.AreEqual(1, registry.Get<int>(entity1));
				}
				else if (entity == entity3)
				{
					NUnit.Framework.Assert.AreEqual(3, registry.Get<int>(entity3));
				}
			}

			registry.Assign<char>(entity1);
			registry.Assign<char>(entity3);

			NUnit.Framework.Assert.AreEqual(0, group.Count);

			registry.Unassign<char>(entity2);
			registry.Unassign<char>(entity4);

			foreach (var entity in group)
			{
				NUnit.Framework.Assert.True(entity == entity2 || entity == entity4);

				if (entity == entity2)
				{
					NUnit.Framework.Assert.AreEqual(2, registry.Get<int>(entity2));
				}
				else if (entity == entity4)
				{
					NUnit.Framework.Assert.AreEqual(4, registry.Get<int>(entity4));
				}
			}
		}

		[Test]
		public void StableLateInitialization()
		{
			var registry = new Registry();

			for (int i = 0; i < 30; i++)
			{
				var entity = registry.CreateEntity();

				if (i % 2 != 0)
				{
					registry.Assign<int>(entity);
				}

				if (i % 3 != 0)
				{
					registry.Assign<char>(entity);
				}
			}

			var group = SetUpWithIntAndWithoutChar(registry);

			NUnit.Framework.Assert.AreEqual(5, group.Count);
		}

		private Group SetUpWithIntAndChar(Registry registry)
		{
			return registry.Group<Include<int, char>>();
		}

		private Group SetUpWithIntAndWithoutChar(Registry registry)
		{
			return registry.Group<Include<int>, Exclude<char>>();

		}
	}
}

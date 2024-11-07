using System;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class ReactiveFilterTests
	{
		[Test]
		public void IncludeFilter()
		{
			var registry = new Registry();

			var reactiveFilter = SetUpWithIntAndChar(registry);

			Assert.AreEqual(0, reactiveFilter.Count);

			var entity1 = registry.CreateEntity('1').Id;
			var entity2 = registry.CreateEntity('2').Id;

			registry.Assign(entity2, 42);

			Assert.AreEqual(1, reactiveFilter.Count);

			registry.Assign<int>(entity1);

			Assert.AreEqual(2, reactiveFilter.Count);

			registry.Unassign<int>(entity1);

			Assert.AreEqual(1, reactiveFilter.Count);

			foreach (var id in reactiveFilter)
			{
				Assert.AreEqual(registry.Get<int>(id), 42);
				Assert.AreEqual(registry.Get<char>(id), '2');
			}

			var enumerator = reactiveFilter.GetEnumerator();
			enumerator.MoveNext();
			Assert.AreEqual(enumerator.Current, entity2);
			enumerator.Dispose();

			registry.Unassign<char>(entity1);
			registry.Unassign<char>(entity2);

			Assert.AreEqual(0, reactiveFilter.Count);
		}

		[Test]
		public void ExcludeFilter()
		{
			var registry = new Registry();

			var entity1 = registry.CreateEntity(1).Id;

			var entity2 = registry.CreateEntity(2).Id;
			registry.Assign<char>(entity2);

			var reactiveFilter = SetUpWithIntAndWithoutChar(registry);

			var entity3 = registry.CreateEntity(3).Id;

			var entity4 = registry.CreateEntity(4).Id;
			registry.Assign<char>(entity4);

			foreach (var entity in reactiveFilter)
			{
				Assert.True(entity == entity1 || entity == entity3);

				if (entity == entity1)
				{
					Assert.AreEqual(1, registry.Get<int>(entity1));
				}
				else if (entity == entity3)
				{
					Assert.AreEqual(3, registry.Get<int>(entity3));
				}
			}

			registry.Assign<char>(entity1);
			registry.Assign<char>(entity3);

			Assert.AreEqual(0, reactiveFilter.Count);

			registry.Unassign<char>(entity2);
			registry.Unassign<char>(entity4);

			foreach (var entity in reactiveFilter)
			{
				Assert.True(entity == entity2 || entity == entity4);

				if (entity == entity2)
				{
					Assert.AreEqual(2, registry.Get<int>(entity2));
				}
				else if (entity == entity4)
				{
					Assert.AreEqual(4, registry.Get<int>(entity4));
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

			var reactiveFilter = SetUpWithIntAndWithoutChar(registry);

			Assert.AreEqual(5, reactiveFilter.Count);
		}

		private ReactiveFilter SetUpWithIntAndChar(Registry registry)
		{
			return registry.ReactiveFilter<Include<int, char>>();
		}

		private ReactiveFilter SetUpWithIntAndWithoutChar(Registry registry)
		{
			return registry.ReactiveFilter<Include<int>, Exclude<char>>();

		}
	}
}

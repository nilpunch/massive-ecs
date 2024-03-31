using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture]
	public class GroupsTests
	{
		[Test]
		public void NonOwningGroup_ShouldWork()
		{
			var registry = new Registry();

			var group = registry.Groups.EnsureGroup(new ISet[]
			{
				registry.Components<int>(),
				registry.Components<char>()
			});
			
			Assert.AreEqual(group.Length, 0);

			var entity1 = registry.Create('1');
			var entity2 = registry.Create('2');

			registry.Add(entity2, 42);
			
			Assert.AreEqual(group.Length, 1);
			
			registry.Add<int>(entity1);

			Assert.AreEqual(group.Length, 2);
			
			registry.Remove<int>(entity1);

			Assert.AreEqual(group.Length, 1);

			foreach (var id in group.Ids)
			{
				Assert.AreEqual(registry.Get<int>(id), 42);
				Assert.AreEqual(registry.Get<char>(id), '2');
			}
			
			Assert.AreEqual(group.Ids[0], entity2);
			
			registry.Remove<char>(entity1);
			registry.Remove<char>(entity2);
			
			Assert.AreEqual(group.Length, 0);
		}

		[Test]
		public void OwningGroup_ShouldWork()
		{
			var registry = new Registry();

			var group = registry.Groups.EnsureGroup(include: new IReadOnlySet[]
			{
				registry.Components<int>(),
				registry.Components<char>()
			});
			
			Assert.AreEqual(group.Length, 0);

			var entity1 = registry.Create('1');
			var entity2 = registry.Create('2');

			registry.Add(entity2, 42);
			
			Assert.AreEqual(group.Length, 1);
			
			registry.Add<int>(entity1);

			Assert.AreEqual(group.Length, 2);
			
			registry.Remove<int>(entity1);

			Assert.AreEqual(group.Length, 1);

			foreach (var id in group.Ids)
			{
				Assert.AreEqual(registry.Get<int>(id), 42);
				Assert.AreEqual(registry.Get<char>(id), '2');
			}
			
			Assert.AreEqual(group.Ids[0], entity2);
			
			registry.Remove<char>(entity1);
			registry.Remove<char>(entity2);
			
			Assert.AreEqual(group.Length, 0);
		}
	}
}
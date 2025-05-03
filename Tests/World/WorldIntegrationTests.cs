using Massive.PerformanceTests;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture(typeof(TestState64), typeof(TestState64_2))]
	[TestFixture(typeof(TestState64Stable), typeof(TestState64))]
	[TestFixture(typeof(TestState64), typeof(TestState64Stable))]
	[TestFixture(typeof(TestState64Stable), typeof(TestState64Stable_2))]
	public class WorldIntegrationTests<TComponent1, TComponent2>
	{
		[Test]
		public void StorageResizeDuringIterationOverComponents_ShouldNotCauseConfusion()
		{
			var world = new World();

			for (int i = 0; i < 1000; i++)
			{
				var entity = world.Create<TComponent1>();
			}

			dynamic iterations = new int();
			world.View().ForEach((ref TComponent1 value) =>
			{
				if (iterations == 0)
				{
					for (int i = 0; i < 2000; i++)
					{
						var entity = world.Create<TComponent1>();
					}

					world.View().ForEach((int id, ref TComponent1 value) =>
					{
						world.Destroy(id);
					});
				}

				iterations += 1;
			});

			Assert.AreEqual(1, iterations);
		}
		
		[Test]
		public void StorageResizeDuringIterationOverComponents_ShouldNotCauseConfusion_v2()
		{
			var world = new World();

			for (int i = 0; i < 1000; i++)
			{
				var entity = world.Create<TComponent1>();
			}

			dynamic iterations = new int();
			world.View().ForEach((ref TComponent1 value) =>
			{
				if (iterations == 0)
				{
					for (int i = 0; i < 2000; i++)
					{
						var entity = world.Create<TComponent1>();
					}

					world.View().ForEach((int id, ref TComponent1 value) =>
					{
						world.Destroy(id);
					});
					
					for (int i = 0; i < 1000; i++)
					{
						var entity = world.Create<TComponent1>();
					}
				}

				iterations += 1;
			});

			Assert.AreEqual(1000, iterations);
		}

		[Test]
		public void StorageResizeDuringIterationOverEntities_ShouldNotCauseConfusion()
		{
			var world = new World();

			for (int i = 0; i < 1000; i++)
			{
				var entity = world.Create<TComponent1>();
			}

			dynamic iterations = new int();
			world.View().ForEach(_ =>
			{
				if (iterations == 0)
				{
					for (int i = 0; i < 2000; i++)
					{
						var entity = world.Create<TComponent1>();
					}

					world.View().ForEach((int id, ref TComponent1 value) =>
					{
						world.Destroy(id);
					});
				}

				iterations += 1;
			});

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void StorageResizeDuringIterationOverEntities_ShouldNotCauseConfusion_v2()
		{
			var world = new World();

			for (int i = 0; i < 1000; i++)
			{
				var entity = world.Create<TComponent1>();
			}

			dynamic iterations = new int();
			foreach (var id in world.View())
			{
				if (iterations == 0)
				{
					for (int i = 0; i < 2000; i++)
					{
						var entity = world.Create<TComponent1>();
					}

					world.View().ForEach((int id, ref TComponent1 value) =>
					{
						world.Destroy(id);
					});
				}

				iterations += 1;
			}

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void DestroyManyEntitiesDuringIterationOverComponents_TheNumberOfIterationsGoesDownTheSame()
		{
			var world = new World();

			for (int i = 0; i < 2000; i++)
			{
				var entity = world.Create<TComponent1>();
				if (i % 2 == 0)
				{
					world.Add<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			world.View().ForEach((ref TComponent2 b) =>
			{
				iterations += 1;
				world.View().ForEach((int innerId, ref TComponent2 innerB) =>
				{
					world.Destroy(innerId);
				});
			});

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void DestroyManyEntitiesDuringIterationOverEntities_TheNumberOfIterationsGoesDownTheSame()
		{
			var world = new World();

			for (int i = 0; i < 2000; i++)
			{
				var entity = world.Create<TComponent1>();
				if (i % 2 == 0)
				{
					world.Add<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			world.View().ForEach(_ =>
			{
				iterations += 1;
				world.View().ForEach((int innerId, ref TComponent2 innerB) =>
				{
					world.Destroy(innerId);
				});
			});

			Assert.IsTrue(1000 == iterations || 1001 == iterations);
		}

		[Test]
		public void DestroyManyEntitiesDuringIterationOverEntities_TheNumberOfIterationsGoesDownTheSame_v2()
		{
			var world = new World();

			for (int i = 0; i < 2000; i++)
			{
				var entity = world.Create<TComponent1>();
				if (i % 2 == 0)
				{
					world.Add<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			foreach (var id in world.View())
			{
				iterations += 1;
				world.View().ForEach((int innerId, ref TComponent2 innerB) =>
				{
					world.Destroy(innerId);
				});
			}

			Assert.IsTrue(1000 == iterations || 1001 == iterations);
		}

		[Test]
		public void RemoveManyComponentsDuringIterationOverComponents_TheNumberOfIterationsGoesDownTheSameAmount()
		{
			var world = new World();
			for (int i = 0; i < 2000; i++)
			{
				var entity = world.Create<TComponent1>();
				if (i % 2 == 0)
				{
					world.Add<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			world.View().ForEach((ref TComponent2 b) =>
			{
				iterations += 1;
				world.View().ForEach((int innerId, ref TComponent2 innerB) =>
				{
					world.Remove<TComponent2>(innerId);
				});
			});

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void RemoveManyComponentsDuringIterationOverComponents_TheNumberOfIterationsGoesDownTheSameAmount_v2()
		{
			var world = new World();
			for (int i = 0; i < 2000; i++)
			{
				var entity = world.Create<TComponent1>();
				if (i % 2 == 0)
				{
					world.Add<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			world.View().ForEach((ref TComponent1 c1, ref TComponent2 c2) =>
			{
				iterations += 1;
				world.View().ForEach((int innerId, ref TComponent2 innerC2) =>
				{
					world.Remove<TComponent2>(innerId);
				});
			});

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void RemoveManyComponentsDuringIterationOverComponents_TheNumberOfIterationsGoesDownTheSameAmount_v3()
		{
			var world = new World();
			for (int i = 0; i < 2000; i++)
			{
				var entity = world.Create<TComponent1>();
				if (i % 2 == 0)
				{
					world.Add<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			world.View().ForEach((ref TComponent1 c1, ref TComponent2 c2) =>
			{
				iterations += 1;
				world.View().ForEach((int innerId, ref TComponent1 innerC2) =>
				{
					world.Remove<TComponent1>(innerId);
				});
			});

			Assert.AreEqual(1, iterations);
		}
	}

	[TestFixture]
	public class WorldIntegrationTests
	{
		[Stable]
		private struct StableData
		{
			public int Value;
		}

		[Test]
		public void RemoveManyEntitiesFromBeginning_WithStable_TheDataDoesNotInvalidating()
		{
			var world = new World();
			for (int i = 0; i <= 2000; i++)
			{
				int entity = world.Create();
				world.Set(entity, new StableData() { Value = entity });
			}

			world.View().ForEach((int entity, ref StableData c2) =>
			{
				Assert.AreEqual(world.Get<StableData>(entity).Value, c2.Value);

				for (int i = 0; i < 1000; i++)
				{
					world.Remove<StableData>(i);
				}

				c2.Value = 1000000;

				Assert.AreEqual(world.Get<StableData>(entity).Value, c2.Value);
			});
		}
	}
}

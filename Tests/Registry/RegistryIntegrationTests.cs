﻿using Massive.PerformanceTests;
using NUnit.Framework;

namespace Massive.Tests
{
	[TestFixture(typeof(TestState64), typeof(TestState64_2))]
	public class RegistryIntegrationTests<TComponent1, TComponent2>
	{
		[Test]
		public void StorageResizeDuringIterationOverComponents_ShouldNotCauseConfusion()
		{
			var registry = new Registry();

			for (int i = 0; i < 1000; i++)
			{
				registry.Create<TComponent1>();
			}

			dynamic iterations = new int();
			registry.View().ForEach((ref TComponent1 value) =>
			{
				if (iterations == 0)
				{
					for (int i = 0; i < 2000; i++)
					{
						registry.Create<TComponent1>();
					}

					registry.View().ForEach((int id, ref TComponent1 value) =>
					{
						registry.Destroy(id);
					});
				}

				iterations += 1;
			});

			Assert.AreEqual(1, iterations);
		}
		
		[Test]
		public void StorageResizeDuringIterationOverComponents_ShouldNotCauseConfusion_v2()
		{
			var registry = new Registry();

			for (int i = 0; i < 1000; i++)
			{
				registry.Create<TComponent1>();
			}

			dynamic iterations = new int();
			registry.View().ForEach((ref TComponent1 value) =>
			{
				if (iterations == 0)
				{
					for (int i = 0; i < 2000; i++)
					{
						registry.Create<TComponent1>();
					}

					registry.View().ForEach((int id, ref TComponent1 value) =>
					{
						registry.Destroy(id);
					});
					
					for (int i = 0; i < 1000; i++)
					{
						registry.Create<TComponent1>();
					}
				}

				iterations += 1;
			});

			Assert.IsTrue(iterations == 1001 || iterations == 1000);
		}

		[Test]
		public void StorageResizeDuringIterationOverEntities_ShouldNotCauseConfusion()
		{
			var registry = new Registry();

			for (int i = 0; i < 1000; i++)
			{
				registry.Create<TComponent1>();
			}

			dynamic iterations = new int();
			registry.View().ForEach(_ =>
			{
				if (iterations == 0)
				{
					for (int i = 0; i < 2000; i++)
					{
						registry.Create<TComponent1>();
					}

					registry.View().ForEach((int id, ref TComponent1 value) =>
					{
						registry.Destroy(id);
					});
				}

				iterations += 1;
			});

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void StorageResizeDuringIterationOverEntities_ShouldNotCauseConfusion_v2()
		{
			var registry = new Registry();

			for (int i = 0; i < 1000; i++)
			{
				registry.Create<TComponent1>();
			}

			dynamic iterations = new int();
			foreach (var id in registry.View())
			{
				if (iterations == 0)
				{
					for (int i = 0; i < 2000; i++)
					{
						registry.Create<TComponent1>();
					}

					registry.View().ForEach((int id, ref TComponent1 value) =>
					{
						registry.Destroy(id);
					});
				}

				iterations += 1;
			}

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void DestroyManyEntitiesDuringIterationOverComponents_TheNumberOfIterationsGoesDownTheSame()
		{
			var registry = new Registry();

			for (int i = 0; i < 2000; i++)
			{
				var entity = registry.Create<TComponent1>();
				if (i % 2 == 0)
				{
					registry.Assign<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			registry.View().ForEach((ref TComponent2 b) =>
			{
				iterations += 1;
				registry.View().ForEach((int innerId, ref TComponent2 innerB) =>
				{
					registry.Destroy(innerId);
				});
			});

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void DestroyManyEntitiesDuringIterationOverEntities_TheNumberOfIterationsGoesDownTheSame()
		{
			var registry = new Registry();

			for (int i = 0; i < 2000; i++)
			{
				var entity = registry.Create<TComponent1>();
				if (i % 2 == 0)
				{
					registry.Assign<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			registry.View().ForEach(_ =>
			{
				iterations += 1;
				registry.View().ForEach((int innerId, ref TComponent2 innerB) =>
				{
					registry.Destroy(innerId);
				});
			});

			Assert.IsTrue(iterations == 1001 || iterations == 1000);
		}

		[Test]
		public void DestroyManyEntitiesDuringIterationOverEntities_TheNumberOfIterationsGoesDownTheSame_v2()
		{
			var registry = new Registry();

			for (int i = 0; i < 2000; i++)
			{
				var entity = registry.Create<TComponent1>();
				if (i % 2 == 0)
				{
					registry.Assign<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			foreach (var id in registry.View())
			{
				iterations += 1;
				registry.View().ForEach((int innerId, ref TComponent2 innerB) =>
				{
					registry.Destroy(innerId);
				});
			}

			Assert.IsTrue(iterations == 1001 || iterations == 1000);
		}

		[Test]
		public void UnassignManyComponentsDuringIterationOverComponents_TheNumberOfIterationsGoesDownTheSameAmount()
		{
			var registry = new Registry();
			for (int i = 0; i < 2000; i++)
			{
				var entity = registry.Create<TComponent1>();
				if (i % 2 == 0)
				{
					registry.Assign<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			registry.View().ForEach((ref TComponent2 b) =>
			{
				iterations += 1;
				registry.View().ForEach((int innerId, ref TComponent2 innerB) =>
				{
					registry.Unassign<TComponent2>(innerId);
				});
			});

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void UnassignManyComponentsDuringIterationOverComponents_TheNumberOfIterationsGoesDownTheSameAmount_v2()
		{
			var registry = new Registry();
			for (int i = 0; i < 2000; i++)
			{
				var entity = registry.Create<TComponent1>();
				if (i % 2 == 0)
				{
					registry.Assign<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			registry.View().ForEach((ref TComponent1 c1, ref TComponent2 c2) =>
			{
				iterations += 1;
				registry.View().ForEach((int innerId, ref TComponent2 innerC2) =>
				{
					registry.Unassign<TComponent2>(innerId);
				});
			});

			Assert.AreEqual(1, iterations);
		}

		[Test]
		public void UnassignManyComponentsDuringIterationOverComponents_TheNumberOfIterationsGoesDownTheSameAmount_v3()
		{
			var registry = new Registry();
			for (int i = 0; i < 2000; i++)
			{
				var entity = registry.Create<TComponent1>();
				if (i % 2 == 0)
				{
					registry.Assign<TComponent2>(entity);
				}
			}

			dynamic iterations = new int();
			registry.View().ForEach((ref TComponent1 c1, ref TComponent2 c2) =>
			{
				iterations += 1;
				registry.View().ForEach((int innerId, ref TComponent1 innerC2) =>
				{
					registry.Unassign<TComponent1>(innerId);
				});
			});

			Assert.AreEqual(1, iterations);
		}
	}
}

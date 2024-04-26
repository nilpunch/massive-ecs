using System.Diagnostics;
using Massive.Samples.Shooter;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public class PhysicsSimulation : MonoBehaviour
	{
		[SerializeField, Range(0.01f, 10f)] private float _simulationSpeed = 1f;
		[SerializeField] private int _simulationsPerFrame = 120;
		[SerializeField] private int _particlesCapacity = 1000;

		[Header("Physics")] [SerializeField] private EntityRoot<PhysicsSphereCollider> _spherePrefab;
		[SerializeField] private EntityRoot<PhysicsBoxCollider> _boxPrefab;
		[SerializeField] private int _substeps = 8;
		[SerializeField] private float _gravity = 10f;

		private MassiveRegistry _registry;
		private IReadOnlyDataSet<PhysicsRigidbody> _bodies;
		private IReadOnlyDataSet<PhysicsSphereCollider> _sphereColliders;
		private IReadOnlyDataSet<PhysicsBoxCollider> _boxColliders;
		private EntitySynchronisation<PhysicsSphereCollider> _spheresSynchronisation;
		private EntitySynchronisation<PhysicsBoxCollider> _boxesSynchronisation;

		private void Awake()
		{
			_registry = new MassiveRegistry(_particlesCapacity, _simulationsPerFrame + 1);
			_spheresSynchronisation = new EntitySynchronisation<PhysicsSphereCollider>(new EntityFactory<PhysicsSphereCollider>(_spherePrefab));
			_boxesSynchronisation = new EntitySynchronisation<PhysicsBoxCollider>(new EntityFactory<PhysicsBoxCollider>(_boxPrefab));

			foreach (var massiveRigidbody in FindObjectsOfType<MassiveRigidbody>())
			{
				massiveRigidbody.Spawn(_registry);
			}

			PhysicsRigidbody.RecalculateAllInertia(_registry.Components<PhysicsRigidbody>(), _registry.Components<PhysicsBoxCollider>(),
				_registry.Components<PhysicsSphereCollider>());

			_bodies = _registry.Components<PhysicsRigidbody>();
			_sphereColliders = _registry.Components<PhysicsSphereCollider>();
			_boxColliders = _registry.Components<PhysicsBoxCollider>();

			_registry.SaveFrame();
		}

		private int _currentFrame;

		private float _elapsedTime;

		private void Update()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			if (_registry.CanRollbackFrames >= 0)
			{
				var previousFrameCount = _currentFrame;
				_currentFrame = Mathf.Max(_currentFrame - _registry.CanRollbackFrames, 0);
				_registry.Rollback(previousFrameCount - _currentFrame);
			}

			_elapsedTime += Time.deltaTime * _simulationSpeed;

			int targetFrame = Mathf.RoundToInt(_elapsedTime * 60);

			while (_currentFrame < targetFrame)
			{
				const float simulationDeltaTime = 1f / 60f;
				float subStepDeltaTime = simulationDeltaTime / _substeps;

				for (int i = 0; i < _substeps; i++)
				{
					PhysicsRigidbody.UpdateAllWorldInertia(_bodies);
					PhysicsSphereCollider.UpdateWorldPositions(_bodies, _sphereColliders);
					PhysicsBoxCollider.UpdateWorldPositions(_bodies, _boxColliders);
					Collisions.Solve(_bodies, _sphereColliders, _boxColliders);
					Gravity.Apply(_bodies, _gravity);
					PhysicsRigidbody.IntegrateAll(_bodies, subStepDeltaTime);
				}

				_registry.SaveFrame();
				_currentFrame++;
			}

			// float systemEnergy = 0f;
			// foreach (var body in _bodies.AliveData)
			// {
			// 	systemEnergy += body.Mass * Mathf.Max(0, body.Position.y + 100f) * _gravity;
			// 	systemEnergy += body.Velocity.sqrMagnitude * body.Mass / 2f;
			// }
			//
			// Debug.Log(systemEnergy);

			_spheresSynchronisation.Synchronize(_sphereColliders);
			_boxesSynchronisation.Synchronize(_boxColliders);

			_debugTime = stopwatch.ElapsedMilliseconds;
		}

		private long _debugTime;

		private void OnGUI()
		{
			float fontScaling = Screen.height / (float)1080;

			GUILayout.TextField($"{_debugTime}ms Simulation",
				new GUIStyle() { fontSize = Mathf.RoundToInt(70 * fontScaling), normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_registry.CanRollbackFrames} Resimulations",
				new GUIStyle() { fontSize = Mathf.RoundToInt(50 * fontScaling), normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_sphereColliders.Count} Spheres",
				new GUIStyle() { fontSize = Mathf.RoundToInt(50 * fontScaling), normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_boxColliders.Count} Boxes",
				new GUIStyle() { fontSize = Mathf.RoundToInt(50 * fontScaling), normal = new GUIStyleState() { textColor = Color.white } });
		}
	}
}

using System;
using System.Diagnostics;
using MassiveData.Samples.Shooter;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace MassiveData.Samples.Physics
{
	public class PhysicsSimulation : MonoBehaviour
	{
		[SerializeField, Range(0.01f, 10f)] private float _simulationSpeed = 1f;
		[SerializeField] private int _simulationsPerFrame = 120;
		[SerializeField] private int _particlesCapacity = 1000;

		[Header("Physics")]
		[SerializeField] private EntityRoot<SphereCollider> _spherePrefab;
		[SerializeField] private EntityRoot<BoxCollider> _boxPrefab;
		[SerializeField] private int _substeps = 8;
		[SerializeField] private float _gravity = 10f;

		private Massive<SphereCollider> _sphereColliders;
		private Massive<BoxCollider> _boxColliders;
		private Massive<Rigidbody> _bodies;
		private EntitySynchronisation<SphereCollider> _spheresSynchronisation;
		private EntitySynchronisation<BoxCollider> _boxesSynchronisation;

		private void Awake()
		{
			_sphereColliders = new Massive<SphereCollider>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			_boxColliders = new Massive<BoxCollider>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			_bodies = new Massive<Rigidbody>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			
			_spheresSynchronisation = new EntitySynchronisation<SphereCollider>(new EntityFactory<SphereCollider>(_spherePrefab));
			_boxesSynchronisation = new EntitySynchronisation<BoxCollider>(new EntityFactory<BoxCollider>(_boxPrefab));

			foreach (var spawnPoint in FindObjectsOfType<PhysicsSpawnPoint>())
			{
				spawnPoint.Spawn(_bodies, _sphereColliders);
			}
		}

		private int _currentFrame;

		private float _elapsedTime;

		private void Update()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			if (_sphereColliders.CanRollbackFrames >= 0)
			{
				_currentFrame -= _sphereColliders.CanRollbackFrames;
				_sphereColliders.Rollback(_sphereColliders.CanRollbackFrames);
				_bodies.Rollback(_bodies.CanRollbackFrames);
			}

			_elapsedTime += Time.deltaTime * _simulationSpeed;

			int targetFrame = Mathf.RoundToInt(_elapsedTime * 60);

			while (_currentFrame < targetFrame)
			{
				const float simulationDeltaTime = 1f / 60f;
				float subStepDeltaTime = simulationDeltaTime / _substeps;
				
				for (int i = 0; i < _substeps; i++)
				{
					SphereCollider.UpdateWorldPositions(_bodies, _sphereColliders);
					BoxCollider.UpdateWorldPositions(_bodies, _boxColliders);
					Collisions.Solve(_sphereColliders, _bodies);
					Gravity.Apply(_bodies, _gravity);
					Rigidbody.IntegrateAll(_bodies, subStepDeltaTime);
				}

				_sphereColliders.SaveFrame();
				_boxColliders.SaveFrame();
				_bodies.SaveFrame();
				_currentFrame++;
			}

			float systemEnergy = 0f;
			foreach (var body in _bodies.AliveData)
			{
				systemEnergy += body.Mass * Mathf.Max(0, body.Position.y + 100f) * _gravity;
				systemEnergy += body.Velocity.sqrMagnitude * body.Mass / 2f;
			}
			
			Debug.Log(systemEnergy);
			
			_spheresSynchronisation.Synchronize(_sphereColliders);
			_boxesSynchronisation.Synchronize(_boxColliders);

			_debugTime = stopwatch.ElapsedMilliseconds;
		}

		private long _debugTime;

		private void OnGUI()
		{
			GUILayout.TextField($"{_debugTime}ms Simulation", new GUIStyle() { fontSize = 70, normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_sphereColliders.CanRollbackFrames} Resimulations", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_sphereColliders.AliveCount} Particles", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
		}
	}
}
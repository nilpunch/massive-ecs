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

		[Header("Physics")] [SerializeField] private EntityRoot<SphereCollider> _particlePrefab;
		[SerializeField] private int _substeps = 8;
		[SerializeField] private float _gravity = 10f;
		[SerializeField] private float _groundFriction = 0.2f;

		private Massive<SphereCollider> _sphereColliders;
		private Massive<Rigidbody> _rigidbodies;
		private EntitySynchronisation<SphereCollider> _particleSynchronisation;

		private void Awake()
		{
			_sphereColliders = new Massive<SphereCollider>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			_rigidbodies = new Massive<Rigidbody>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			
			_particleSynchronisation = new EntitySynchronisation<SphereCollider>(new EntityFactory<SphereCollider>(_particlePrefab));

			foreach (var spawnPoint in FindObjectsOfType<PhysicsSpawnPoint>())
			{
				spawnPoint.Spawn(_rigidbodies, _sphereColliders);
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
				_rigidbodies.Rollback(_rigidbodies.CanRollbackFrames);
			}

			_elapsedTime += Time.deltaTime * _simulationSpeed;

			int targetFrame = Mathf.RoundToInt(_elapsedTime * 60);

			while (_currentFrame < targetFrame)
			{
				const float simulationDeltaTime = 1f / 60f;
				float subStepDeltaTime = simulationDeltaTime / _substeps;
				

				for (int i = 0; i < _substeps; i++)
				{
					SphereCollider.UpdateWorldPositions(_rigidbodies, _sphereColliders);
					Collisions.Solve(_rigidbodies, _sphereColliders);
					Gravity.Apply(_rigidbodies, _gravity);
					Rigidbody.IntegrateAll(_rigidbodies, subStepDeltaTime);
				}

				_sphereColliders.SaveFrame();
				_rigidbodies.SaveFrame();
				_currentFrame++;
			}

			_particleSynchronisation.Synchronize(_sphereColliders);

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
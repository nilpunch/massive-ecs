using System;
using System.Diagnostics;
using Massive.Samples.Shooter;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Massive.Samples.Physics
{
	public class PhysicsSimulation : MonoBehaviour
	{
		[SerializeField, Range(0.01f, 10f)] private float _simulationSpeed = 1f;
		[SerializeField] private int _simulationsPerFrame = 120;
		[SerializeField] private int _particlesCapacity = 1000;

		[Header("Physics")] [SerializeField] private EntityRoot<PointMass> _particlePrefab;
		[SerializeField] private int _substeps = 8;
		[SerializeField] private float _gravity = 10f;
		[SerializeField] private float _groundFriction = 0.2f;

		private MassiveData<PointMass> _particles;
		private MassiveData<SoftBody> _softBodies;
		private MassiveData<Spring> _springs;
		private EntitySynchronisation<PointMass> _particleSynchronisation;

		private void Awake()
		{
			_particles = new MassiveData<PointMass>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			_softBodies = new MassiveData<SoftBody>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			_springs = new MassiveData<Spring>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			
			_particleSynchronisation = new EntitySynchronisation<PointMass>(new EntityFactory<PointMass>(_particlePrefab));

			foreach (var spawnPoint in FindObjectsOfType<PhysicsSpawnPoint>())
			{
				spawnPoint.Spawn(_softBodies, _particles, _springs);
			}
		}

		private int _currentFrame;

		private float _elapsedTime;

		private void Update()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();

			if (_particles.CanRollbackFrames >= 0)
			{
				_currentFrame -= _particles.CanRollbackFrames;
				_particles.Rollback(_particles.CanRollbackFrames);
				_springs.Rollback(_springs.CanRollbackFrames);
				_softBodies.Rollback(_softBodies.CanRollbackFrames);
			}

			_elapsedTime += Time.deltaTime * _simulationSpeed;

			int targetFrame = Mathf.RoundToInt(_elapsedTime * 60);

			while (_currentFrame < targetFrame)
			{
				const float simulationDeltaTime = 1f / 60f;
				float subStepDeltaTime = simulationDeltaTime / _substeps;
				

				for (int i = 0; i < _substeps; i++)
				{
					SoftBody.UpdateAll(_softBodies, _particles);
					ReferenceSpringing.Apply(_particles, _softBodies, subStepDeltaTime);
					Gravity.Apply(_particles, _gravity);
					GlobalFloor.Apply(_particles, frictionCoefficient: _groundFriction);
					Spring.ApplyAll(_springs, _particles, subStepDeltaTime);
					PointMass.IntegrateAll(_particles, subStepDeltaTime);
				}

				_particles.SaveFrame();
				_springs.SaveFrame();
				_softBodies.SaveFrame();
				_currentFrame++;
			}

			_particleSynchronisation.Synchronize(_particles);

			_debugTime = stopwatch.ElapsedMilliseconds;
		}

		private long _debugTime;

		private void OnGUI()
		{
			GUILayout.TextField($"{_debugTime}ms Simulation", new GUIStyle() { fontSize = 70, normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_particles.CanRollbackFrames} Resimulations", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_particles.AliveCount} Particles", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_springs.AliveCount} Springs", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
		}
	}
}
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
		
		[Header("Physics")]
		[SerializeField] private EntityRoot<Particle> _particlePrefab;
		[SerializeField] private int _substeps = 8;
		[SerializeField] private float _gravity = 10f;
		[SerializeField] private float _groundFriction = 0.2f;

		private MassiveData<Particle> _particles;
		private MassiveData<Spring> _springs;
		private EntitySynchronisation<Particle> _particleSynchronisation;

		private void Awake()
		{
			_particles = new MassiveData<Particle>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			_springs = new MassiveData<Spring>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			_particleSynchronisation = new EntitySynchronisation<Particle>(new EntityFactory<Particle>(_particlePrefab));

			foreach (var particleSpawnPoint in FindObjectsOfType<ParticleSpawnPoint>())
			{
				_particles.Create(new Particle(particleSpawnPoint.Position, particleSpawnPoint.Radius, 1f, particleSpawnPoint.Drag));
			}
			
			foreach (var spawnPoint in FindObjectsOfType<PhysicsSpawnPoint>())
			{
				spawnPoint.Spawn(_particles.CurrentFrame, _springs.CurrentFrame);
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
			}

			_elapsedTime += Time.deltaTime * _simulationSpeed;
			
			int targetFrame = Mathf.RoundToInt(_elapsedTime * 60);

			while (_currentFrame < targetFrame)
			{
				var particles = _particles.CurrentFrame;
				var springs = _springs.CurrentFrame;

				const float simulationDeltaTime = 1f / 60f;
				float subStepDeltaTime = simulationDeltaTime / _substeps;

				for (int i = 0; i < _substeps; i++)
				{
					Gravity.Apply(particles, _gravity);
					Collisions.Solve(particles);
					GlobalFloorConstraint.Apply(particles, frictionCoefficient: _groundFriction);
					Spring.ApplyAll(springs, particles, subStepDeltaTime);
					Particle.IntegrateAll(particles, subStepDeltaTime);
				}
				
				_particles.SaveFrame();
				_springs.SaveFrame();
				_currentFrame++;
			}
			
			_particleSynchronisation.Synchronize(_particles.CurrentFrame);

			_debugTime = stopwatch.ElapsedMilliseconds;
		}

		private long _debugTime;
		
		private void OnGUI()
		{
			GUILayout.TextField($"{_debugTime}ms Simulation", new GUIStyle() { fontSize = 70, normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_particles.CanRollbackFrames} Resimulations", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_particles.CurrentFrame.AliveCount} Particles", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
			GUILayout.TextField($"{_springs.CurrentFrame.AliveCount} Springs", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
		}
	}
}
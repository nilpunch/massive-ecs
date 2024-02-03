using System;
using System.Diagnostics;
using Massive.Samples.Shooter;
using UnityEngine;

namespace Massive.Samples.Physics
{
	public class PhysicsSimulation : MonoBehaviour
	{
		[SerializeField] private int _simulationsPerFrame = 120;
		[SerializeField] private int _particlesCapacity = 1000;
		
		[Header("Physics")]
		[SerializeField] private EntityRoot<Particle> _particlePrefab;
		[SerializeField] private int _substeps = 8;
		[SerializeField] private float _gravity = 10f;

		private MassiveData<Particle> _particles;
		private EntitySynchronisation<Particle> _particleSynchronisation;

		private void Awake()
		{
			_particles = new MassiveData<Particle>(framesCapacity: _simulationsPerFrame, dataCapacity: _particlesCapacity);
			_particleSynchronisation = new EntitySynchronisation<Particle>(new EntityFactory<Particle>(_particlePrefab));

			foreach (var particleSpawnPoint in FindObjectsOfType<ParticleSpawnPoint>())
			{
				_particles.Create(new Particle(particleSpawnPoint.Position, particleSpawnPoint.Radius));
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
			}

			_elapsedTime += Time.deltaTime;
			
			int targetFrame = Mathf.RoundToInt(_elapsedTime * 60);

			while (_currentFrame < targetFrame)
			{
				var particles = _particles.CurrentFrame;

				const float simulationDeltaTime = 1f / 60f;
				float subStepDeltaTime = simulationDeltaTime / _substeps;

				for (int i = 0; i < _substeps; i++)
				{
					Gravity.Apply(particles, _gravity);
					Collisions.Solve(particles);
					GlobalFloorConstaint.Apply(particles);
					Particle.Update(particles, subStepDeltaTime);
				}
				
				_particles.SaveFrame();
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
		}
	}
}
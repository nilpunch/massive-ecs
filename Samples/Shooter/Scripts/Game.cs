using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Massive.Samples.Shooter
{
	public class Game : MonoBehaviour
	{
		[SerializeField] private int _simulationsPerFrame = 120;
		[SerializeField] private int _charactersCapacity = 10;
		[SerializeField] private int _bulletsCapacity = 1000;
		
		private MassiveData<CharacterState> _characters;
		private MassiveData<BulletState> _bullets;
		private WorldUpdater[] _worldUpdaters;

		private void Awake()
		{
			_characters = new MassiveData<CharacterState>(dataCapacity: _charactersCapacity);
			_bullets = new MassiveData<BulletState>(dataCapacity: _bulletsCapacity);

			_worldUpdaters = FindObjectsOfType<WorldUpdater>();

			for (int i = 0; i < _charactersCapacity; i++)
			{
				_characters.Create();
			}
		}

		private void Update()
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			
			for (int i = 0; i < _simulationsPerFrame; i++)
			{
				var world = new WorldFrame(_characters.CurrentFrame, _bullets.CurrentFrame, Time.frameCount, 60);
			
				foreach (var worldUpdater in _worldUpdaters)
				{
					worldUpdater.UpdateWorld(world);
				}
			
				_characters.SaveFrame();
				_bullets.SaveFrame();
			}

			_debugTime = stopwatch.ElapsedMilliseconds;
		}

		private long _debugTime;
		
		private void OnGUI()
		{
			GUILayout.TextField($"{_debugTime}ms", new GUIStyle() { fontSize = 50, normal = new GUIStyleState() { textColor = Color.white } });
		}
	}
}
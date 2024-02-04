using UnityEngine;

namespace Massive.Samples.Shooter
{
	public class FPSCounter : MonoBehaviour
	{
		private int[] _frameRateSamples;
		private readonly int _averageFromAmount = 60;

		private int _averageCounter;
		private int _currentAveraged;

		void Awake()
		{
			_frameRateSamples = new int[_averageFromAmount];
		}

		void OnGUI()
		{
			var currentFrame = (int)Mathf.Round(1f / Time.smoothDeltaTime);
			_frameRateSamples[_averageCounter] = Mathf.Max(currentFrame, 0);

			var average = 0f;
			foreach (var frameRate in _frameRateSamples)
			{
				average += frameRate;
			}

			_currentAveraged = (int)Mathf.Round(average / _averageFromAmount);
			_averageCounter = (_averageCounter + 1) % _averageFromAmount;

			GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));

			GUILayout.BeginHorizontal();

			GUILayout.FlexibleSpace();

			GUILayout.TextField($"{_currentAveraged} FPS", new GUIStyle() { fontSize = 70, normal = new GUIStyleState() { textColor = Color.white } });

			GUILayout.EndHorizontal();

			GUILayout.EndArea();
		}
	}
}
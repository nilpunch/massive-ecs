using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

namespace Massive.Samples.Benchmark
{
    public abstract class MonoProfiler : MonoBehaviour
    {
        private const int SampleCount = 32;

        [SerializeField] public string _identifier;
        [SerializeField] public int _drawOrder;
        [SerializeField, Min(1)] public int _iterations = 1000;

        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly RingBuffer<long> _samples = new RingBuffer<long>(SampleCount);
        private readonly Lazy<GUIStyle> _style = new Lazy<GUIStyle>(() => new GUIStyle("label") { fontSize = 48, alignment = TextAnchor.MiddleCenter });

        protected abstract void Sample();

        private void Update()
        {
            _stopwatch.Restart();
            // Profiler.BeginSample(_identifier);
            for (int i = 0; i < _iterations; i++)
                Sample();
            // Profiler.EndSample();
            _stopwatch.Stop();
            _samples.Push(_stopwatch.ElapsedMilliseconds);
        }

        private void OnGUI()
        {
            var height = (float)Screen.height / 4;
            var area = new Rect(0, _drawOrder * height, Screen.width, height);

            GUI.Label(area, $"{_identifier}: {Average(_samples)}", _style.Value);
        }

        private static float Average(RingBuffer<long> buffer)
        {
            long total = 0;

            for (int i = 0; i < buffer.Length; i++)
            {
                total += buffer[i];
            }

            return (float)total / buffer.Length;
        }
    }
}

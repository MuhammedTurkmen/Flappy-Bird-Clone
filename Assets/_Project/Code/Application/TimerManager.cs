using MET.Core.Patterns;
using UnityEngine;

namespace MET.Core.Applications
{
    public class TimerManager : Singleton<TimerManager>
    {
        public static float SlowFactor = 1;
        public static float Delta => SlowFactor * Time.deltaTime;

        public float ElapsedTime => _elapsedTime;

        private float _elapsedTime;

        private void Update()
        {
            _elapsedTime += Delta;
        }
    }
}

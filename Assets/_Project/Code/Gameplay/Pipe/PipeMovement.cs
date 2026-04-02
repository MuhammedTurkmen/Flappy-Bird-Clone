using MET.Core;
using MET.Core.Applications;
using UnityEngine;

namespace MET.Gameplay
{
    public class PipeMovement : PoolObject
    {
        [SerializeField] private float _movementSpeed;

        private float _startX = 4.5f;

        private int[] _yPoses = { -1, 0, 1 };

        private void OnEnable()
        {
            //transform.position = new Vector3(_startX, _yPoses[Random.Range(0, _yPoses.Length - 1)], 0);
        }

        private void OnDisable()
        {
            transform.position = new Vector3(_startX, _yPoses[Random.Range(0, _yPoses.Length - 1)], 0);

        }

        void Update()
        {
            transform.position -= _movementSpeed * TimerManager.Delta * transform.right;

            if (transform.position.x <= -_startX)
            {
                PipePool.Me.Release(this);
            }
        }
    }
}

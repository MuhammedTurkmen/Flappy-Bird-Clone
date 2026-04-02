using UnityEngine;

namespace MET.Core
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class ReferenceResolutionCamera : MonoBehaviour
    {
        [SerializeField] private float referenceWidth = 1080f;
        [SerializeField] private float referenceHeight = 1920f;

        [Space]

        [SerializeField] private float referenceOrthographicSize = 5f;

        private Camera _camera;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            Apply();
        }

        private void OnEnable()
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();

            Apply();
        }

#if UNITY_EDITOR
        private void Update()
        {
            Apply();
        }
#endif

        private void Apply()
        {
            if (_camera == null || !_camera.orthographic)
                return;

            float referenceAspect = referenceWidth / referenceHeight;
            float currentAspect = (float)Screen.width / Screen.height;

            if (currentAspect >= referenceAspect)
            {
                _camera.orthographicSize = referenceOrthographicSize;
            }
            else
            {
                _camera.orthographicSize = referenceOrthographicSize * (referenceAspect / currentAspect);
            }
        }
    }
}

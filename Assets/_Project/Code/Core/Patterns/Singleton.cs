using UnityEngine;

namespace MET.Core.Patterns
{
    [DefaultExecutionOrder(-900)]
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        [SerializeField] private bool _doDontDestroyOnLoad;

        [SerializeField] private Transform[] _protectedObjects;

        public static T Me
        {
            get
            {
                return _me;
            }
        }

        private static T _me;

        protected virtual void Awake()
        {
            if (_me != null && _me != (T)(object)this) { Debug.Log(gameObject.name); Destroy(gameObject); return; }
            _me = (T)(object)this;

            if (_doDontDestroyOnLoad)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);

                if (_protectedObjects == null || _protectedObjects.Length == 0)
                    return;

                foreach (var item in _protectedObjects)
                {
                    DontDestroyOnLoad(item.root.gameObject);
                }
            }
        }
    }

    [DefaultExecutionOrder(-900)]
    public class Singleton<T, TInterface> : MonoBehaviour
        where T : MonoBehaviour, TInterface
        where TInterface : class
    {
        [SerializeField] private bool _doDontDestroyOnLoad;

        [SerializeField] private Transform[] _protectedObjects;

        public static TInterface Me
        {
            get
            {
                return _me;
            }
        }

        private static T _me;

        protected virtual void Awake()
        {
            if (_me != null && _me != (T)(object)this) { Debug.Log(gameObject.name); Destroy(gameObject); return; }
            _me = (T)(object)this;

            //if (typeof(T) == typeof(SegmentSpawner)) print("segment spawner");

            if (_doDontDestroyOnLoad)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);

                if (_protectedObjects == null || _protectedObjects.Length == 0)
                    return;

                foreach (var item in _protectedObjects)
                {
                    DontDestroyOnLoad(item.root.gameObject);
                }
            }
        }
    }
}
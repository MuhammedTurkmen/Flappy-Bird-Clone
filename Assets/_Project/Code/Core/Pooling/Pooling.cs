using System.Collections;
using System.Collections.Generic;
using MET.Core.Attributes;
using MET.Core.Patterns;
using UnityEngine;
using UnityEngine.Pool;

namespace MET.Core.Pooling
{
    public class Pooling<T> : Singleton<Pooling<T>> where T : MonoBehaviour
    {
        [System.Serializable]
        private class PoolData
        {
            public T Prefab;
            public ObjectPool<T> Pool;
        }

        [Separator("Pool Prefabs")]
        [SerializeField] private T[] _poolPrefabs;

        [Separator("Pool Settings")]
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxSize = 50;
        [SerializeField] private int _prewarmCount = 5;

        private readonly List<T> _activeGameObjects = new();
        private readonly List<PoolData> _pools = new();
        private readonly Dictionary<T, ObjectPool<T>> _instanceToPool = new();

        protected override void Awake()
        {
            base.Awake();

            InitializePools();
            PrewarmPools();
        }

        // =========
        // INITIALIZE
        // =========

        private void InitializePools()
        {
            _pools.Clear();

            foreach (var prefab in _poolPrefabs)
            {
                if (prefab == null)
                    continue;

                T localPrefab = prefab;

                PoolData poolData = new PoolData
                {
                    Prefab = localPrefab
                };

                poolData.Pool = new ObjectPool<T>(
                    createFunc: () => CreateItem(localPrefab),
                    actionOnGet: OnGet,
                    actionOnRelease: OnRelease,
                    actionOnDestroy: OnDestroyItem,
                    collectionCheck: false,
                    defaultCapacity: _defaultCapacity,
                    maxSize: _maxSize
                );

                _pools.Add(poolData);
            }
        }

        private void PrewarmPools()
        {
            foreach (var poolData in _pools)
            {
                List<T> tempItems = new();

                for (int i = 0; i < _prewarmCount; i++)
                {
                    T item = poolData.Pool.Get();
                    tempItems.Add(item);
                }

                foreach (var item in tempItems)
                {
                    poolData.Pool.Release(item);
                }
            }
        }

        // =========
        // PRIVATE
        // =========

        private T CreateItem(T prefab)
        {
            T item = Instantiate(prefab, transform);
            _instanceToPool[item] = GetPoolByPrefab(prefab);
            return item;
        }

        private ObjectPool<T> GetPoolByPrefab(T prefab)
        {
            foreach (var poolData in _pools)
            {
                if (poolData.Prefab == prefab)
                    return poolData.Pool;
            }

            return null;
        }

        private void OnGet(T item)
        {
            item.gameObject.SetActive(true);
            _activeGameObjects.Add(item);
        }

        private void OnRelease(T item)
        {
            item.gameObject.SetActive(false);

            if (item is PoolObject poolObject)
                poolObject.ResetObj();

            _activeGameObjects.Remove(item);
        }

        private void OnDestroyItem(T item)
        {
            _instanceToPool.Remove(item);
            Destroy(item.gameObject);
        }

        private IEnumerator ReturnAfter(T item, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            Release(item);
        }

        // =========
        // PUBLIC
        // =========

        public T Get()
        {
            if (_pools.Count == 0)
            {
                Debug.LogError("No pool prefabs assigned!");
                return null;
            }

            int randomIndex = Random.Range(0, _pools.Count);
            return _pools[randomIndex].Pool.Get();
        }

        public void Release(T item)
        {
            if (item == null) return;

            if (_instanceToPool.TryGetValue(item, out var pool))
            {
                pool.Release(item);
            }
            else
            {
                Debug.LogWarning($"{item.name} does not belong to any pool.");
                Destroy(item.gameObject);
            }
        }

        public void ReleaseAll()
        {
            var copy = new List<T>(_activeGameObjects);

            foreach (var item in copy)
            {
                Release(item);
            }
        }

        public void ReleaseAfter(T item, float seconds)
        {
            StartCoroutine(ReturnAfter(item, seconds));
        }
    }
}
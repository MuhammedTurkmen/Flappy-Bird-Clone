using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace MET.Applications.Events
{
    public class GameEventListener : MonoBehaviour
    {
        [SerializeField] public GameEventsList[] list;

        private readonly List<(Type type, Delegate del)> _subscriptions = new();

        //private void OnEnable()
        //{
        //    SubscribeAll();
        //}

        private void Start()
        {
            SubscribeAll();
        }

        private void OnDisable()
        {
            //UnsubscribeAll();
        }

        private void SubscribeAll()
        {
            foreach (var item in list)
            {
                var type = GetAllEventTypes()
                    .FirstOrDefault(t => t.FullName == item._eventTypeName);

                if (type == null) continue;

                var method = typeof(GameEventListener)
                    .GetMethod(nameof(OnEventGeneric), BindingFlags.NonPublic | BindingFlags.Instance)
                    .MakeGenericMethod(type);

                var del = Delegate.CreateDelegate(
                    typeof(Action<>).MakeGenericType(type),
                    this,
                    method
                );

                var subscribeMethod = typeof(EventBus)
                    .GetMethod(nameof(EventBus.Subscribe))
                    .MakeGenericMethod(type);

                subscribeMethod.Invoke(EventBus.Me, new object[] { del });

                _subscriptions.Add((type, del));
            }
        }

        private void UnsubscribeAll()
        {
            foreach (var sub in _subscriptions)
            {
                var unsubscribeMethod = typeof(EventBus)
                    .GetMethod(nameof(EventBus.Unsubscribe))
                    .MakeGenericMethod(sub.type);

                unsubscribeMethod.Invoke(EventBus.Me, new object[] { sub.del });
            }

            _subscriptions.Clear();
        }

        private void OnEventGeneric<T>(T e) where T : IGameEvent
        {
            var typeName = typeof(T).FullName;

            var item = list.FirstOrDefault(x => x._eventTypeName == typeName);
            item?.Event?.Invoke();
        }

        // TYPE CACHE
        private static List<Type> _cachedTypes;

        public static List<Type> GetAllEventTypes()
        {
            if (_cachedTypes != null)
                return _cachedTypes;

            _cachedTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(IGameEvent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            return _cachedTypes;
        }
    }

    [Serializable]
    public class GameEventsList
    {
        public string _eventTypeName;
        public UnityEvent Event;
    }
}
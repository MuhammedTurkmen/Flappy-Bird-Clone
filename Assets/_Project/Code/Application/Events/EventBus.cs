using System;
using System.Collections.Generic;
using MET.Core.Patterns;
using UnityEngine;

namespace MET.Applications.Events
{
    [DefaultExecutionOrder(-1000)]
    public sealed class EventBus : Singleton<EventBus, IEventBus>, IEventBus
    {
        private readonly Dictionary<Type, object> _subscribers = new();

        public void Subscribe<T>(Action<T> listener)
            where T : IGameEvent
        {
            var type = typeof(T);

            if (_subscribers.TryGetValue(type, out var existing))
            {
                _subscribers[type] = (Action<T>)existing + listener;
            }
            else
            {
                _subscribers[type] = listener;
            }
        }

        public void Unsubscribe<T>(Action<T> listener)
            where T : IGameEvent
        {
            var type = typeof(T);

            if (_subscribers.TryGetValue(type, out var existing))
            {
                var current = (Action<T>)existing - listener;

                if (current == null)
                    _subscribers.Remove(type);
                else
                    _subscribers[type] = current;
            }
        }

        public void Publish<T>(T gameEvent)
            where T : IGameEvent
        {
            var type = typeof(T);

            if (_subscribers.TryGetValue(type, out var existing))
            {
                ((Action<T>)existing)?.Invoke(gameEvent);
            }
        }
    }
}
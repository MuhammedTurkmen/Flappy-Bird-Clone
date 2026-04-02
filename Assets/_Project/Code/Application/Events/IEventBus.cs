using System;

namespace MET.Applications.Events
{
    public interface IEventBus
    {
        public void Subscribe<T>(Action<T> listener) where T : IGameEvent;

        public void Unsubscribe<T>(Action<T> listener) where T : IGameEvent;

        public void Publish<T>(T gameEvent) where T : IGameEvent;
    }
}
using System;

namespace ConsolePilot.Dispatch
{
    public interface IGenericMessageBus
    {
        void Publish<T>(T message);

        void Subscribe<T>(Action<T> callback);

        void Unsubscribe<T>(Action<T> callback);
    }
}

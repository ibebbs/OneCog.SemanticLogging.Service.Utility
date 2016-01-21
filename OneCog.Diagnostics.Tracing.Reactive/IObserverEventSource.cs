using System;

namespace OneCog.Diagnostics.Tracing.Reactive
{
    public interface IObserverEventSource<T>
    {
        void OnNext(T item);

        void OnError(Exception ex);

        void OnComplete();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace OneCog.Diagnostics.Tracing.Reactive
{
    public static class InstrumentedObserver
    {
        public static IObserver<T> Create<T>(IObserver<T> observer, IObserverEventSource<T> eventSource)
        {
            return new InstrumentedObserver<T>(observer.OnNext, observer.OnError, observer.OnCompleted, eventSource);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, Action onCompleted, IObserverEventSource<T> eventSource)
        {
            return new InstrumentedObserver<T>(onNext, onError, onCompleted, eventSource);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, IObserverEventSource<T> eventSource)
        {
            return new InstrumentedObserver<T>(onNext, null, null, eventSource);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action onCompleted, IObserverEventSource<T> eventSource)
        {
            return new InstrumentedObserver<T>(onNext, null, onCompleted, eventSource);
        }

        public static IObserver<T> Create<T>(Action<T> onNext, Action<Exception> onError, IObserverEventSource<T> eventSource)
        {
            return new InstrumentedObserver<T>(onNext, onError, null, eventSource);
        }

        public static IDisposable SubscribeWithInstrumentation<T>(this IObservable<T> source, IObserver<T> observer, IObserverEventSource<T> eventSource)
        {
            return source.Subscribe(new InstrumentedObserver<T>(observer.OnNext, observer.OnError, observer.OnCompleted, eventSource));
        }

        public static IDisposable SubscribeWithInstrumentation<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, Action onCompleted, IObserverEventSource<T> eventSource)
        {
            return source.Subscribe(new InstrumentedObserver<T>(onNext, onError, onCompleted, eventSource));
        }

        public static IDisposable SubscribeWithInstrumentation<T>(this IObservable<T> source, Action<T> onNext, IObserverEventSource<T> eventSource)
        {
            return source.Subscribe(new InstrumentedObserver<T>(onNext, null, null, eventSource));
        }

        public static IDisposable SubscribeWithInstrumentation<T>(this IObservable<T> source, Action<T> onNext, Action onCompleted, IObserverEventSource<T> eventSource)
        {
            return source.Subscribe(new InstrumentedObserver<T>(onNext, null, onCompleted, eventSource));
        }

        public static IDisposable SubscribeWithInstrumentation<T>(this IObservable<T> source, Action<T> onNext, Action<Exception> onError, IObserverEventSource<T> eventSource)
        {
            return source.Subscribe(new InstrumentedObserver<T>(onNext, onError, null, eventSource));
        }
    }

    public class InstrumentedObserver<T> : IObserver<T>
    {
        private readonly Action _onCompleted;
        private readonly Action<Exception> _onError;
        private readonly Action<T> _onNext;
        private readonly IObserverEventSource<T> _eventSource;

        public InstrumentedObserver(Action<T> onNext, Action<Exception> onError, Action onCompleted, IObserverEventSource<T> eventSource)
        {
            if (eventSource == null)
            {
                throw new ArgumentNullException("An IObservableEventSource<T> must be specified", "eventSource");
            }

            _onNext = onNext ?? (item => { });
            _onError = onError ?? (ex => { });
            _onCompleted = onCompleted ?? (() => { });
            _eventSource = eventSource;
        }

        void IObserver<T>.OnCompleted()
        {
            try
            {
                _eventSource.OnComplete();
            }
            finally
            {
                _onCompleted();
            }
        }

        void IObserver<T>.OnError(Exception error)
        {
            try
            {
                _eventSource.OnError(error);
            }
            finally
            {
                _onError(error);
            }
        }

        void IObserver<T>.OnNext(T value)
        {
            try
            {
                _eventSource.OnNext(value);
            }
            finally
            {
                _onNext(value);
            }
        }
    }
}

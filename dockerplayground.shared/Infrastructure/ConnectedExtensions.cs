using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public static class ConnectedExtensions
    {
        public static IObservable<IConnected<TResult>> LaunchOrKill<TSource, TResult>(
            this IObservable<IConnected<TSource>> source,
            Func<TSource, TResult> selector) where TResult : IDisposable
        {
            return source.Select(s => GetInstanceStream(() => Select(s, selector)))
                         .Switch();
        }

        private static IObservable<IConnected<T>> GetInstanceStream<T>(Func<IConnected<T>> factory)
        where T : IDisposable
        {
            return Observable.Create<IConnected<T>>(obs =>
            {
                var instance = factory();
                obs.OnNext(instance);
                return instance.IsConnected ? Disposable.Create(() => instance.Value.Dispose()) : Disposable.Empty;
            });
        }

        public static IConnected<TResult> Select<TSource, TResult>(this IConnected<TSource> source,
                                                           Func<TSource, TResult> selector)
        {
            if (source.IsConnected)
                return new Connected<TResult>(selector(source.Value));

            return new Connected<TResult>();
        }

    }
}

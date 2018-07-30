using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace DockerPlayground.Shared.Dao
{
    public static class ObservableExtensions
    {
        public static IObservable<TSource> TakeUntilInclusive<TSource>(this IObservable<TSource> source, Func<TSource, bool> predicate)
        {
            return Observable.Create<TSource>(
                observer => source.Subscribe(
                    item =>
                    {
                        observer.OnNext(item);
                        if (predicate(item))
                            observer.OnCompleted();
                    },
                    observer.OnError,
                    observer.OnCompleted
                    )
                );
        }
    }
}

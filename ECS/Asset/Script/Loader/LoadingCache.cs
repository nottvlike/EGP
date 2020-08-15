namespace Asset
{
    using System;
    using UniRx;
    using UnityEngine;

    internal class LoadingCache<T> : IObserver<T>, IDisposable
    {
        AsyncSubject<T> subject = new AsyncSubject<T>();

        public void Dispose() => subject.Dispose();

        public void OnCompleted() => subject.OnCompleted();

        public void OnError(Exception e) => subject.OnError(e);

        public void OnNext(T cache) => subject.OnNext(cache);

        public IObservable<T> ToLoadingObserable()
        {
            return Observable.Defer(() =>
            {
                return subject.Select(cache => cache);
            });
        }
    }
}
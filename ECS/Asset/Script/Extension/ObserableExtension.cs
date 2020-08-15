namespace ECS.Common
{
    using UniRx;
    using System;
    using System.Collections.Generic;

    internal static class ObserableExtension
    {
        public static IObservable<T> ReportOnComplete<T>(this IObservable<T> observable, IProgress<float> progress = null)
        {
            if (observable == null) return null;
            if (progress == null) return observable;
            return observable.DoOnCompleted(() => progress.Report(1));
        }

        public static IEnumerable<T> AsEnumerable<T>(this T source)
        {
            yield return source;
        }
    }
}
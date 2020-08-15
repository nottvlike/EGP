namespace ECS.Helper
{
    using UniRx;
    using System;

    public interface IDownloadHandler
    {
        string Name { get; }
        string Url { get; }

        float Size { get; }

        bool IsFetchHead { get; }
        bool IsDownload { get; }

        IObservable<Unit> FetchHead();
        IObservable<Unit> Download();

        void Save();
    }
}
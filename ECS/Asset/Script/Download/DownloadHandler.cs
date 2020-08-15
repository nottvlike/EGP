namespace ECS.Helper
{
    using UniRx;
    using System;
    using UnityEngine.Networking;
    using ECS;

    public abstract class DownloadHandler<T> : IDownloadHandler
    {
        public string Name { get; protected set; }
        public string Url { get; protected set; }

        public float Size { get; protected set; }

        public bool IsFetchHead { get; protected set; }
        public bool IsDownload { get; protected set; }

        public DownloadHandler(string url, string name = null)
        {
            Url = url;
            Name = name;

            IsFetchHead = false;
            IsDownload = false;
        }

        public IObservable<Unit> FetchHead()
        {
            return UnityWebRequest.Head(Url).SendAsObserable()
                .ContinueWith(operation =>
                {
                    IsFetchHead = true;
                    var req = operation.webRequest;
                    var lengthStr = req.GetResponseHeader(AssetConstant.HTTP_FILE_LENGTH_FLAG);
                    if (!string.IsNullOrEmpty(lengthStr))
                    {
                        var length = long.Parse(req.GetResponseHeader(AssetConstant.HTTP_FILE_LENGTH_FLAG));
                        Size = (float)length / (AssetConstant.SIZE_KB * AssetConstant.SIZE_KB);
                    }

                    return Observable.ReturnUnit();
                });
        }

        public abstract IObservable<Unit> Download();
        public abstract void Save();

        protected T downloadAsset;
        public IObservable<T> GetAsset()
        {
            return Observable.EveryUpdate().AsUnitObservable().Where(_ => IsDownload)
                .FirstOrDefault().Select(_ => downloadAsset);
        }
    }
}
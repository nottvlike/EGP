namespace ECS.Helper
{
    using ECS;
    using ECS.Data;
    using UniRx;
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public enum DownloadStateType
    {
        None,
        Fetch,
        Download
    }

    public class AssetDownloadBatcher
    {
        public DownloadStateType DownloadStateType { get; private set; }
        public int AllCount { get; private set; }
        public int CurrentCount { get; private set; }

        public float AllSize { get; private set; }
        public float CurrentSize { get; private set; }

        ISubject<int> _downloadSubject = new Subject<int>();
        List<AssetDownloadHandler> _downloadHandlerList = new List<AssetDownloadHandler>();

        public AssetDownloadBatcher(IEnumerable<string> assetNameList)
        {
            var assetCoreUnit = WorldManager.Instance.Unit.GetUnit(AssetConstant.ASSET_CORE_UNIT_NAME);
            var assetCoreData = assetCoreUnit.GetData<AssetProcessData>();

            foreach (var assetName in assetNameList)
            {
                var assetUrl = Path.Combine(assetCoreData.CDN, assetName);
                var assetHash = assetCoreData.Manifest.GetAssetBundleHash(assetName);
                if (!Caching.IsVersionCached(assetUrl, assetHash))
                {
                    _downloadHandlerList.Add(new AssetDownloadHandler(assetUrl, assetName));
                }
            }
        }

        ~AssetDownloadBatcher()
        {
            _downloadSubject.OnCompleted();
            _downloadHandlerList.Clear();
        }

        public IObservable<Unit> Begin(int maxConcurrent = 0)
        {
            var downloadHandlerList = _downloadHandlerList.Where(_ => !_.IsCached).ToArray();
            if (downloadHandlerList.Length <= 0)
            {
                return Observable.ReturnUnit();
            }

            DownloadStateType = DownloadStateType.Fetch;
            var taskData = DataPool.Get<TaskData>();
            taskData.maxConcurrent = maxConcurrent == 0 ? taskData.maxConcurrent : maxConcurrent;
            foreach (var downloadHandler in downloadHandlerList)
            {
                if (downloadHandler.IsFetchHead)
                {
                    continue;
                }
                taskData.taskList.Add(downloadHandler.FetchHead()
                    .Do(_ =>
                    {
                        CurrentCount++;
                        _downloadSubject.OnNext(0);
                    }));
            }

            DownloadStateType = DownloadStateType.Fetch;
            AllCount = taskData.taskList.Count;
            CurrentCount = 0;
            AllSize = CurrentSize = 0f;

            return taskData.taskList.Merge(taskData.maxConcurrent).AsUnitObservable()
                .CatchIgnore()
                .DoOnCompleted(() =>
                {
                    AllCount = CurrentCount = 0;
                    taskData.taskList.Clear();
                    foreach (var downloadHandler in downloadHandlerList)
                    {
                        if (downloadHandler.IsDownload)
                        {
                            continue;
                        }

                        AllSize += downloadHandler.Size;

                        downloadHandler.Clear();
                        taskData.taskList.Add(downloadHandler.Download()
                            .Do(_ =>
                            {
                                CurrentCount++;
                                CurrentSize += downloadHandler.Size;
                                _downloadSubject.OnNext(0);
                            }));
                    }
                }).ContinueWith(_ =>
                {
                    DownloadStateType = DownloadStateType.Download;
                    AllCount = taskData.taskList.Count;
                    if (taskData.taskList.Count > 0)
                    {
                        return taskData.taskList.Merge(taskData.maxConcurrent).AsUnitObservable()
                            .CatchIgnore();
                    }
                    else
                    {
                        return Observable.ReturnUnit();
                    }
                });
        }

        public IObservable<int> ObserveDownloadState()
        {
            return _downloadSubject;
        }
    }
}

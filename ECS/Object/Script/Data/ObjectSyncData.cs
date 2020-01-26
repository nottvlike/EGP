namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using UniRx;
    using UnityEngine;
    using System;

    public class ObjectSyncServerData : IData
    {
        public ReactiveCollection<ServerSyncStateInfo> preparedSyncInfoList = new ReactiveCollection<ServerSyncStateInfo>();
        public IDisposable nextFrameDispose;

        public ReactiveProperty<bool> enable = new ReactiveProperty<bool>(false);
        public int serverKeyFrame;
        public int currentKeyFrame;
        public int internalFrame;
        public int internalFrameSize = 5;

        public ISubject<ValueTuple<int, int>> syncSubject = new Subject<ValueTuple<int, int>>();
    }

    public struct ServerSyncStateInfo
    {
        public uint unitId;
        public string stateName;
        public Vector3 stateParam;
        public ObjectStateType stateType;
    }

    public struct SyncStateInfo
    {
        public int serverKeyFrame;
        public int internalFrame;

        public string stateName;
        public Vector3 stateParam;
        public ObjectStateType stateType;
    }

    public class ObjectSyncData : IData, IPoolObject
    {
        public ReactiveCollection<SyncStateInfo> stateSyncInfoList = new ReactiveCollection<SyncStateInfo>();
        public IDisposable checkSyncDispose;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            stateSyncInfoList.Clear();
            checkSyncDispose = null;
        }
    }
}
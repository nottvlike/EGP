namespace ECS.Data
{
    using ECS.Common;
    using UniRx;
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class ObjectSyncServerData : IData
    {
        public ReactiveCollection<ServerSyncStateInfo> preparedSyncInfoList = new ReactiveCollection<ServerSyncStateInfo>();
        public List<ServerSyncStateInfo> cachedSyncInfoList = new List<ServerSyncStateInfo>();

        public ReactiveProperty<bool> enable = new ReactiveProperty<bool>(false);
        public int serverKeyFrame;
        public int currentKeyFrame;
        public int internalFrame;
        public int internalFrameSize = 6;

        public ISubject<ValueTuple<int, int>> syncSubject = new Subject<ValueTuple<int, int>>();
    }

    public struct ServerSyncStateInfo
    {
        public int serverKeyFrame;

        public uint unitId;
        public int stateId;
        public Vector3 stateParam;
        public ObjectStateType stateType;
    }

    public struct SyncStateCountInfo
    {
        public int serverKeyFrame;
        public int internalFrame;

        public uint unitId;
        public int count;
    }

    public struct SyncStateInfo
    {
        public int serverKeyFrame;
        public int internalFrame;

        public int stateId;
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
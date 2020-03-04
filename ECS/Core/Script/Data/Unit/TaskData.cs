namespace ECS.Data
{
    using UniRx;
    using System;
    using System.Collections.Generic;
    using ECS.Common;

    public class TaskData : IData, IPoolObject
    {
        public int maxConcurrent = 8;
        public int minCheckFinishInternal = 0;
        public IDisposable checkFinishDispose;
        public List<IObservable<Unit>> taskList = new List<IObservable<Unit>>();

        public bool IsInUse { get; set; }
        public void Clear()
        {
            maxConcurrent = 8;
            minCheckFinishInternal = 0;
            checkFinishDispose = null;
            taskList.Clear();
        }
    }
}
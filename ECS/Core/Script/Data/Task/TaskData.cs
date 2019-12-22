namespace ECS.Data
{
    using UniRx;
    using System;
    using System.Collections.Generic;
    using ECS.Common;

    public class TaskData : IData, IPoolObject
    {
        public List<IObservable<Unit>> taskList = new List<IObservable<Unit>>();

        public bool IsInUse { get; set; }
        public void Clear()
        {
            taskList.Clear();
        }
    }
}
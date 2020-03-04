namespace ECS.Module
{
    using Data;
    using System;
    using GUnit = ECS.Unit.Unit;
    using UniRx;
    using UnityEngine;

    public sealed class TaskModule : Module
    {
        public static void Start(TaskData taskData, Action onFinished = null)
        {
            if (taskData.taskList.Count > 0)
            {
                var hasMinCheckFinishInterval = taskData.minCheckFinishInternal > 0;
                var isFinish = false;
                taskData.taskList.Merge(taskData.maxConcurrent).AsUnitObservable().Finally(() =>
                {
                    taskData.taskList.Clear();

                    isFinish = true;
                    if (!hasMinCheckFinishInterval)
                    {
                        onFinished?.Invoke();
                    }
                }).Subscribe();

                if (hasMinCheckFinishInterval)
                {
                    taskData.checkFinishDispose = Observable.Interval(
                        TimeSpan.FromMilliseconds(taskData.minCheckFinishInternal)).Subscribe(_ =>
                    {
                        if (isFinish)
                        {
                            taskData.checkFinishDispose?.Dispose();
                            taskData.checkFinishDispose = null;

                            onFinished?.Invoke();
                        }
                    });
                }
            }
            else
            {
                onFinished?.Invoke();
            }
        }
    }
}
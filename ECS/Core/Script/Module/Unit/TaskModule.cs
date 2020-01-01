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
                taskData.taskList.Merge(taskData.maxConcurrent).AsUnitObservable().Finally(() =>
                {
                    taskData.taskList.Clear();
                    onFinished?.Invoke();
                }).Subscribe();
            }
            else
            {
                onFinished?.Invoke();
            }
        }
    }
}
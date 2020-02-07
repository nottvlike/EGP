namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Object.Data;
    using UniRx;
    using UnityEngine;
    using System.Linq;

    public sealed class ObjectStateProcess : Module
    {
        static bool ContainExcludeState(string[] nameList, string stateName)
        {
            if (nameList == null)
            {
                return false;
            }

            for (var i = 0; i < nameList.Length; i++)
            {
                if (nameList[i] == stateName)
                {
                    return true;
                }
            }

            return false;
        }

        static bool CanStart(ObjectStateProcessData stateProcessData, ObjectStateData stateData, Vector3 param)
        {
            return stateProcessData.currentState == null
                || stateData.isSupporting
                || (!ContainExcludeState(stateProcessData.currentState.excludeNameList, stateData.name)
                && ((stateProcessData.currentState == stateData && stateData.param != param)
                || (stateProcessData.currentState != stateData && stateData.priority >= stateProcessData.currentState.priority)));
        }

        public static void Start(GUnit unit, string name, Vector3 param, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = stateProcessData.allStateList.Where(_ => _.name == name).FirstOrDefault();

            if (!CanStart(stateProcessData, stateData, param))
            {
                return;
            }

            if (sync && unit.GetData<ObjectSyncData>() == null)
            {
                sync = false;
            }

            if (sync)
            {
                ObjectSyncServer.AddState(unit, name, param, ObjectStateType.Start);
            }
            else
            {
                stateData.param = param;
                Start(stateProcessData, stateData);
            }
        }

        static bool CanUpdate(ObjectStateProcessData stateProcessData, ObjectStateData stateData)
        {
            // return stateProcessData.currentState == stateData 
            //     || stateProcessData.stopStateList.IndexOf(stateData) != -1;
            return true;
        }

        public static void Update(GUnit unit, string name, Vector3 param, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = stateProcessData.allStateList.Where(_ => _.name == name).FirstOrDefault();

            if (!CanUpdate(stateProcessData, stateData))
            {
                return;
            }

            if (sync && unit.GetData<ObjectSyncData>() == null)
            {
                sync = false;
            }

            if (sync)
            {
                ObjectSyncServer.AddState(unit, name, param, ObjectStateType.Update);
            }
            else
            {
                stateData.param = param;
            }
        }

        static bool CanFinish(ObjectStateProcessData stateProcessData, ObjectStateData stateData)
        {
            return true;
        }

        public static void Finish(GUnit unit, string name, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = stateProcessData.allStateList.Where(_ => _.name == name).FirstOrDefault();

            if (!CanFinish(stateProcessData, stateData))
            {
                return;
            }

            if (sync && unit.GetData<ObjectSyncData>() == null)
            {
                sync = false;
            }

            if (sync)
            {
                ObjectSyncServer.AddState(unit, name, Vector3.zero, ObjectStateType.Finish);
            }
            else
            {
                Finish(stateProcessData, stateData);
            }
        }

        static float GetCurrentStateProcess(ObjectStateProcessData stateData)
        {
            return 0f;
        }

        static void Start(ObjectStateProcessData stateProcessData, ObjectStateData stateData)
        {
            if (!stateData.isSupporting)
            {
                if (stateProcessData.currentState != null)
                {
                    Stop(stateProcessData);
                }

                stateData.stateTypeProperty.Value = ObjectStateType.Start;
                stateProcessData.currentState = stateData;

                if (!stateData.isLoop)
                {
                    stateProcessData.checkFinishDispose?.Dispose();
                    stateProcessData.checkFinishDispose = Observable.EveryUpdate().Do(_ => 
                    {
                        var process = GetCurrentStateProcess(stateProcessData);
                        if (process >= 1f)
                        {
                            Finish(stateProcessData);
                            stateProcessData.checkFinishDispose.Dispose();
                        }
                    }).Subscribe();
                }
            }
            else
            {
                stateData.stateTypeProperty.SetValueAndForceNotify(ObjectStateType.Start);
            }
        }

        static void Stop(ObjectStateProcessData stateProcessData)
        {
            var currentState = stateProcessData.currentState;
            currentState.stateTypeProperty.Value = ObjectStateType.Stop;

            stateProcessData.stopStateList.Add(stateProcessData.currentState);
            stateProcessData.currentState = null;
        }

        static void Finish(ObjectStateProcessData stateProcessData, ObjectStateData stateData = null)
        {
            var currentState = stateData == null ? stateProcessData.currentState : stateData;
            if (currentState == null)
            {
                return;
            }

            if (!currentState.isSupporting)
            {
                if (currentState.stateTypeProperty.Value == ObjectStateType.Start)
                {
                    currentState.stateTypeProperty.Value = ObjectStateType.Finish;
                    stateProcessData.currentState = null;

                    var newStateData = GetHighestPriorityState(stateProcessData);
                    stateProcessData.stopStateList.Remove(newStateData);

                    Start(stateProcessData, newStateData);
                }
                else if (currentState.stateTypeProperty.Value == ObjectStateType.Stop)
                {
                    currentState.stateTypeProperty.Value = ObjectStateType.Finish;
                    stateProcessData.stopStateList.Remove(stateData);
                }
            }
            else
            {
                currentState.stateTypeProperty.Value = ObjectStateType.Finish;
            }
        }

        static ObjectStateData GetHighestPriorityState(ObjectStateProcessData stateProcessData)
        {
            var stopStateList = stateProcessData.stopStateList;

            var minPriority = 0;
            ObjectStateData result = null;
            foreach (var stateData in stopStateList)
            {
                if (stateData.priority >= minPriority)
                {
                    minPriority = stateData.priority;
                    result = stateData;
                }
            }

            return result;
        }
    }
}
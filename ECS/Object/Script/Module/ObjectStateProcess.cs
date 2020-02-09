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
        public static void Start(GUnit unit, uint id, Vector3 param, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = stateProcessData.allStateList.Where(_ => _.id == id).FirstOrDefault();

            if (!stateData.objectState.CanStart(unit, stateProcessData, stateData, param))
            {
                return;
            }

            if (sync && unit.GetData<ObjectSyncData>() == null)
            {
                sync = false;
            }

            if (sync)
            {
                ObjectSyncServer.AddState(unit, id, param, ObjectStateType.Start);
            }
            else
            {
                stateData.param = param;
                Start(stateProcessData, stateData);
            }
        }

        public static void Update(GUnit unit, uint id, Vector3 param, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = stateProcessData.allStateList.Where(_ => _.id == id).FirstOrDefault();

            if (!stateData.objectState.CanUpdate(unit, stateProcessData, stateData, param))
            {
                return;
            }

            if (sync && unit.GetData<ObjectSyncData>() == null)
            {
                sync = false;
            }

            if (sync)
            {
                ObjectSyncServer.AddState(unit, id, param, ObjectStateType.Update);
            }
            else
            {
                stateData.param = param;
            }
        }

        public static void Finish(GUnit unit, uint id, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = stateProcessData.allStateList.Where(_ => _.id == id).FirstOrDefault();

            if (!stateData.objectState.CanFinish(unit, stateProcessData, stateData))
            {
                return;
            }

            if (sync && unit.GetData<ObjectSyncData>() == null)
            {
                sync = false;
            }

            if (sync)
            {
                ObjectSyncServer.AddState(unit, id, Vector3.zero, ObjectStateType.Finish);
            }
            else
            {
                Finish(unit, stateProcessData, stateData);
            }
        }

        static void Start(ObjectStateProcessData stateProcessData, ObjectStateData stateData)
        {
            if (stateData is IndependentObjectStateData)
            {
                if (stateProcessData.currentState != null)
                {
                    Stop(stateProcessData);
                }

                stateData.stateTypeProperty.Value = ObjectStateType.Start;
                stateProcessData.currentState = stateData as IndependentObjectStateData;
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

            stateProcessData.currentState = null;
        }

        static void Finish(GUnit unit, ObjectStateProcessData stateProcessData, ObjectStateData stateData = null)
        {
            var currentState = stateData == null ? stateProcessData.currentState : stateData;
            if (currentState == null)
            {
                return;
            }

            if (currentState is IndependentObjectStateData)
            {
                if (currentState.stateTypeProperty.Value == ObjectStateType.Start)
                {
                    currentState.stateTypeProperty.Value = ObjectStateType.Finish;
                    stateProcessData.currentState = null;

                    var newStateData = GetHighestPriorityState(unit, stateProcessData);
                    Start(stateProcessData, newStateData);
                }
                else if (currentState.stateTypeProperty.Value == ObjectStateType.Stop)
                {
                    currentState.stateTypeProperty.Value = ObjectStateType.Finish;
                }
            }
            else
            {
                currentState.stateTypeProperty.Value = ObjectStateType.Finish;
            }
        }

        static IndependentObjectStateData GetHighestPriorityState(GUnit unit, ObjectStateProcessData stateProcessData)
        {
            var stateList = stateProcessData.allStateList.Where(_ => _ is IndependentObjectStateData);

            var minPriority = 0;
            IndependentObjectStateData result = null;
            foreach (var stateData in stateList)
            {
                if (stateData.stateTypeProperty.Value != ObjectStateType.Stop)
                {
                    continue;
                }

                if (!stateData.objectState.CanStart(unit, stateProcessData, stateData, Vector3.zero))
                {
                    continue;
                }

                var independentStateData = stateData as IndependentObjectStateData;
                if (independentStateData.priority >= minPriority)
                {
                    minPriority = independentStateData.priority;
                    result = independentStateData;
                }
            }

            return result;
        }
    }
}
namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using ECS.Common;
    using UniRx;
    using UnityEngine;
    using System;
    using System.Linq;

    public sealed class ObjectStateProcess : Module
    {
        public override int Group { get; protected set; }
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.STATE_MODULE_GROUP_NAME);

        public ObjectStateProcess()
        {
            RequiredDataList = new Type[]
            {
                typeof(ObjectStateProcessData)
            };
        }

        protected override void OnAdd(GUnit unit)
        {
            var stateDataList = ObjectStateDataDict.Get(unit);
            foreach (var stateData in stateDataList)
            {
                var stateModule = ObjectStateModuleDict.Get(stateData.id);
                stateData.objectState = stateModule;

                stateModule.Init(unit, stateData);
            }
        }

        protected override void OnRemove(GUnit unit)
        {
            var stateDataList = ObjectStateDataDict.Get(unit);
            foreach (var stateData in stateDataList)
            {
                stateData.objectState.Release(unit, stateData);
                stateData.objectState = null;

                Pool.Release(stateData);
            }

            ObjectStateDataDict.Clear(unit);
        }


        public static void Start(GUnit unit, int id, Vector3 param, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = ObjectStateDataDict.Get(unit, id);

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

        public static void Update(GUnit unit, int id, Vector3 param, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = ObjectStateDataDict.Get(unit, id);

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

        public static void Finish(GUnit unit, int id, bool sync = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var stateData = ObjectStateDataDict.Get(unit, id);

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
            var stateDataList = ObjectStateDataDict.Get(unit).Where(_ => _ is IndependentObjectStateData);

            var minPriority = 0;
            IndependentObjectStateData result = null;
            foreach (var stateData in stateDataList)
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
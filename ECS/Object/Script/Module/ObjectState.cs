namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Data;
    using ECS.Object.Data;
    using UniRx;
    using UnityEngine;

    public interface IObjectState
    {
        bool CanStart(GUnit unit, ObjectStateProcessData stateProcessData, ObjectStateData stateData, Vector3 param);
        bool CanUpdate(GUnit unit, ObjectStateProcessData stateProcessData, ObjectStateData stateData, Vector3 param);
        bool CanFinish(GUnit unit, ObjectStateProcessData stateProcessData, ObjectStateData stateData);
    }

    public abstract class ObjectState<T> : Module, IObjectState where T : ObjectStateData
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.STATE_MODULE_GROUP_NAME);

        protected override void OnAdd(GUnit unit)
        {
            var stateData = GetStateData(unit);
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var unitData = unit.GetData<UnitData>();
            stateData.stateTypeProperty.Subscribe(_ => {
                if (_ == ObjectStateType.Start)
                {
                    OnStart(unit, stateData);
                }
                else if (_ == ObjectStateType.Stop)
                {
                    OnStop(unit, stateData);
                }
                else if (_ == ObjectStateType.Finish)
                {
                    OnFinish(unit, stateData);
                    stateData.stateTypeProperty.Value = ObjectStateType.None;
                }
            }).AddTo(unitData.disposable);

            stateProcessData.allStateList.Add(stateData);

            OnInit(unit, stateData);

            var independentStateData = stateData as IndependentObjectStateData;
            if (independentStateData != null && independentStateData.isDefault)
            {
                ObjectStateProcess.Start(unit, independentStateData.id, independentStateData.param, false);
            }
        }

        protected override void OnRemove(GUnit unit)
        {
            var stateData = GetStateData(unit);
            OnRelease(unit, stateData);

            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            stateData.stateTypeProperty.Value = ObjectStateType.Finish;
            stateProcessData.allStateList.Remove(stateData);
        }

        protected T GetStateData(GUnit unit)
        {
            return unit.GetData<T>();
        }

        bool ContainExcludeState(uint[] nameList, uint id)
        {
            if (nameList == null)
            {
                return false;
            }

            for (var i = 0; i < nameList.Length; i++)
            {
                if (nameList[i] == id)
                {
                    return true;
                }
            }

            return false;
        }

        public virtual bool CanStart(GUnit unit, ObjectStateProcessData stateProcessData, ObjectStateData stateData, Vector3 param)
        {
            if (stateData is IndependentObjectStateData)
            {
                if (stateProcessData.currentState == null)
                {
                    return true;
                }

                var independentState = stateData as IndependentObjectStateData;
                var isSameState = stateProcessData.currentState == independentState;
                var containExcludeState = ContainExcludeState(stateProcessData.currentState.excludeIdList, stateData.id);
                var higherPriority = independentState.priority >= stateProcessData.currentState.priority;
                return isSameState && independentState.param != param
                    || !isSameState && !containExcludeState && higherPriority;
            }
            else
            {
                return true;
            }
        }

        public virtual bool CanUpdate(GUnit unit, ObjectStateProcessData processData, ObjectStateData stateData, Vector3 param)
        {
            return true;
        }

        public virtual bool CanFinish(GUnit unit, ObjectStateProcessData processData, ObjectStateData stateData)
        {
            return true;
        }

        protected virtual void OnInit(GUnit unit, T stateData)
        {
            stateData.objectState = this;
        }

        protected virtual void OnRelease(GUnit unit, T stateData)
        {
        }

        protected abstract void OnStart(GUnit unit, T stateData);
        protected abstract void OnStop(GUnit unit, T stateData);
        protected abstract void OnFinish(GUnit unit, T stateData);
    }
}
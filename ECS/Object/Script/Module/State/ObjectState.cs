namespace ECS.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Data;
    using UniRx;
    using UnityEngine;

    public abstract class ObjectState
    {
        public virtual int Id { get; }

        public virtual int[] ExcludeIdList { get; }

        bool ContainExcludeState(int[] nameList, int id)
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
                var currentState = stateProcessData.currentState;
                if (currentState == null)
                {
                    return true;
                }

                var independentState = stateData as IndependentObjectStateData;
                var isSameState = currentState == independentState;
                var containExcludeState = ContainExcludeState(currentState.objectState.ExcludeIdList, stateData.id);
                var higherPriority = independentState.priority >= currentState.priority;
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

        public void Init(GUnit unit, ObjectStateData stateData)
        {
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

            OnInit(unit, stateData);

            var independentStateData = stateData as IndependentObjectStateData;
            if (independentStateData != null && independentStateData.isDefault)
            {
                ObjectStateProcess.Start(unit, independentStateData.id, independentStateData.param, false);
            }
        }

        public void Release(GUnit unit, ObjectStateData stateData)
        {
            if (stateData.stateTypeProperty.Value == ObjectStateType.None
                || stateData.stateTypeProperty.Value == ObjectStateType.Finish)
            {
                return;
            }

            stateData.stateTypeProperty.Value = ObjectStateType.Finish;

            OnRelease(unit, stateData);
        }

        protected virtual void OnInit(GUnit unit, ObjectStateData stateData) { }
        protected virtual void OnStart(GUnit unit, ObjectStateData stateData) { }
        protected virtual void OnStop(GUnit unit, ObjectStateData stateData) { }
        protected virtual void OnFinish(GUnit unit, ObjectStateData stateData) { }
        protected virtual void OnRelease(GUnit unit, ObjectStateData stateData) { }
    }
}
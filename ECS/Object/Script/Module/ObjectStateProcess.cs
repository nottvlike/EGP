namespace ECS.Object.Module
{
    using GUnit = ECS.Unit.Unit;
    using ECS.Module;
    using ECS.Object.Data;
    using UniRx;

    public class ObjectStateProcess : Module
    {
        public override int Group { get; protected set; } 
            = WorldManager.Instance.Module.TagToModuleGroupType(ObjectConstant.STATE_MODULE_GROUP_NAME);

        public static void Start(GUnit unit, ObjectStateData stateData)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            if (stateProcessData.currentState == stateData || 
                (stateProcessData.currentState != null && stateProcessData.currentState.priority > stateData.priority))
            {
                return;
            }

            if (stateProcessData.currentState == null)
            {
                stateProcessData.currentState = stateData;
            }
            else
            {
                Stop(stateProcessData);

                stateData.stateTypeProperty.Value = ObjectStateType.Start;
                stateProcessData.currentState = stateData;
            }

            if (!stateData.isLoop)
            {
                stateProcessData.checkFinishDispose.Dispose();
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

        public static void Finish(GUnit unit, ObjectStateData stateData, bool startNewState = true)
        {
            var stateProcessData = unit.GetData<ObjectStateProcessData>();
            var currentState = stateData.stateTypeProperty.Value;
            if (currentState == ObjectStateType.Start)
            {
                Finish(stateProcessData, startNewState);
            }
            else if (currentState == ObjectStateType.Stop)
            {
                stateProcessData.stopStateList.Remove(stateData);
                stateData.stateTypeProperty.Value = ObjectStateType.Finish;
            }
        }

        static float GetCurrentStateProcess(ObjectStateProcessData stateData)
        {
            return 0f;
        }

        static void Stop(ObjectStateProcessData stateData)
        {
            var currentState = stateData.currentState;
            currentState.stateTypeProperty.Value = ObjectStateType.Stop;

            stateData.stopStateList.Add(stateData.currentState);
            stateData.currentState = null;
        }

        static void Finish(ObjectStateProcessData stateProcessData, bool startNewState = true)
        {
            if (stateProcessData.currentState == null || stateProcessData.currentState.isLoop)
            {
                return;
            }

            var currentState = stateProcessData.currentState;
            currentState.stateTypeProperty.Value = ObjectStateType.Finish;
            stateProcessData.currentState = null;

            if (startNewState)
            {
                var stateData = GetHighestPriorityState(stateProcessData);
                stateProcessData.stopStateList.Remove(stateData);
                stateProcessData.currentState = stateData;
                stateData.stateTypeProperty.Value = ObjectStateType.Start;
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
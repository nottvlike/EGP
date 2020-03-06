namespace ECS.Module
{
    using Data;
    using System;
    using System.Collections.Generic;
    using GUnit = ECS.Unit.Unit;
    using UniRx;
    using UnityEngine;

    public sealed class GameState : SingleModule
    {
        public override int Group { get; protected set; }
            = WorldManager.Instance.Module.TagToModuleGroupType(Constant.SYSTEM_MODULE_GROUP_NAME);

        public GameState()
        {
            RequiredDataList = new Type[]{
                typeof(GameStateData)
            };
        }

        static GameStateData _stateData;
        protected override void OnAdd(GUnit unit)
        {
            _stateData = unit.GetData<GameStateData>();
            _stateData.currentState = new ReactiveProperty<int>();
        }

        protected override void OnRemove(GUnit unit)
        {
            base.OnRemove(unit);

            _stateData.currentState.Dispose();
            _stateData = null;
        }

        public static void Start(int state)
        {
            _stateData.currentState.Value = state;
        }

        public static IObservable<int> ObserveGameState(int targetState = 0)
        {
            return Observable.Defer(() =>
            {
                if (targetState == 0)
                {
                    return _stateData.currentState;
                }
                else
                {
                    return _stateData.currentState.Where(state => state == targetState);
                }
            });
        }
    }
}
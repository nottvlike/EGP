using ECS;
using ECS.Data;
using ECS.Module;
using Game.ObjectTest.Module;
using Game.ObjectTest.Factory;
using UnityEngine;
using UniRx;

public class PlayerControlTest : GameStart
{
    protected override void StartGame() 
    {
        var moduleMgr = WorldManager.Instance.Module;
        var gameCore = WorldManager.Instance.Unit.GetUnit(Constant.GAME_CORE_UNIT_NAME);
        gameCore.UpdateRequiredModuleGroup(moduleMgr.TagToModuleGroupType(Constant.SYNC_MODULE_GROUP_NAME));
        var syncServerData = gameCore.AddData<ObjectSyncServerData>();
        gameCore.UpdateMeetModuleList();

        var factory = WorldManager.Instance.Factory;

        var slowDownTrapObject = GameObject.Find("Ground/SlowDownTrap");
        factory.CreateSlowDownTrap(slowDownTrapObject);

        AssetProcess.Load<GameObject>("Prefabs/Cube").Subscribe(asset => 
        {
            factory.CreatePlayer(asset);
            syncServerData.enable.Value = true;
        });
    }

    protected override void RegisterGameModule()
    {
        var moduleMgr = WorldManager.Instance.Module;
        moduleMgr.Register(new ObjectSyncLocalServer());
        moduleMgr.Register(new ObjectSyncServer());
        moduleMgr.Register(new ObjectSync());
        moduleMgr.Register(new ObjectBuffProcess());
        moduleMgr.Register(new ObjectStateProcess());
        moduleMgr.Register(new ObjectControlState());
#if UNITY_EDITOR || UNITY_STANDALONE
        moduleMgr.Register(new ObjectKeyboardControlProcess());
#endif
#if UNITY_ANDROID || UNITY_IPHONE
        moduleMgr.Register(new ObjectUIControlProcess());
#endif

        moduleMgr.Register(new SlowDownTrap());
    }
}
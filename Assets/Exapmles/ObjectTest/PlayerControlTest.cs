using ECS;
using ECS.Object.Data;
using ECS.Object.Module;
using ECS.Object;
using Game.ObjectTest.Module;
using Game.ObjectTest.Module.Control;
using Game.ObjectTest.Module.State;
using Game.ObjectTest.Factory;
using UnityEngine;
using UniRx;
using Asset;

public class PlayerControlTest : GameStart
{
    protected override void StartGame() 
    {
        var moduleMgr = WorldManager.Instance.Module;
        var gameCore = WorldManager.Instance.Unit.GetUnit(Constant.GAME_CORE_UNIT_NAME);
        gameCore.UpdateRequiredModuleGroup(moduleMgr.TagToModuleGroupType(ObjectConstant.SYNC_MODULE_GROUP_NAME));
        var syncServerData = gameCore.AddData<ObjectSyncServerData>();
        gameCore.UpdateMeetModuleList();

        AssetManager.Load<GameObject>("Prefabs/Cube").Subscribe(asset => 
        {
            WorldManager.Instance.Factory.CreatePlayer(asset);
            syncServerData.enable.Value = true;
        });
    }

    protected override void RegisterGameModule()
    {
        var moduleMgr = WorldManager.Instance.Module;
        moduleMgr.Register(new ObjectSyncLocalServer());
        moduleMgr.Register(new ObjectSyncServer());
        moduleMgr.Register(new ObjectSync());

        moduleMgr.Register(new ObjectMoveLeft());
        moduleMgr.Register(new ObjectMoveRight());
        moduleMgr.Register(new ObjectFinishMoveLeft());
        moduleMgr.Register(new ObjectFinishMoveRight());
        moduleMgr.Register(new ObjectIdle());
        moduleMgr.Register(new ObjectMove());
    }
}
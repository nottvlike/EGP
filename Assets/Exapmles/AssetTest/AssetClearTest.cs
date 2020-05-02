using ECS.Common;
using ECS.Module;
using UniRx;
using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class AssetClearTest : GameStart
{
    CompositeDisposable disposables = new CompositeDisposable();

    void OnDestroy() 
    {
        disposables.Dispose();
    }

    protected override void StartGame() 
    {
        // spawn / despwn 接口只能用 asset资源（不能是实例化出来的资源）调用，为了统一管理资源！
        AssetProcess.Load<GameObject>("Prefabs/Actor/Plane1").Subscribe(asset => 
        {
            asset.Spawn();
        });

        AssetProcess.Load<GameObject>("Prefabs/Actor/Cube1").Subscribe(asset => 
        {
            var cubeList = new List<GameObject>();
            for (var i = 0; i < 15; i++)
            {
                var cubeAsset = asset.Spawn();
                cubeAsset.name = "cube_" + i;
                cubeAsset.transform.position = new Vector3(URandom.Range(0, 10), 
                    URandom.Range(0, 10), URandom.Range(0, 10));
                cubeList.Add(cubeAsset);
            }

            for (var i = 10; i < 15; i++)
            {
                cubeList[i].Despawn();
            }

            var cubeArray = cubeList.ToArray();
            AssetProcess.ClearUnusedAsset();
            Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(_ => 
            {     
                Log.I("Cube list fter clear unused asset!");
                foreach (var cube in cubeArray)
                {
                    Log.I("cube {0}", cube);
                }

                AssetProcess.ClearByAssetPrefix("Prefabs/Actor");
                Observable.Timer(TimeSpan.FromSeconds(2)).Subscribe(time => 
                {
                    Log.I("Cube list fter clear asset by prefix {0}!", "Prefabs/Actor");
                    foreach (var cube in cubeArray)
                    {
                        Log.I("cube {0}", cube);
                    } 
                });
            });
        });
    }

    protected override void RegisterGameModule()
    {
    }
}
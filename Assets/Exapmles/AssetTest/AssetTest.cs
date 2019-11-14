using ECS.Common;
using Asset;
using UniRx;
using System;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class AssetTest : GameStart
{
    CompositeDisposable disposables = new CompositeDisposable();

    void OnDestroy() 
    {
        disposables.Dispose();
    }

    protected override void StartGame() 
    {
        // spawn / despwn 接口只能用 asset资源（不能是实例化出来的资源）调用，为了统一管理资源！
        AssetManager.Load<GameObject>("Prefabs/Actor/Plane1").Subscribe(asset => 
        {
            asset.Spawn();
        });

        AssetManager.Load<GameObject>("Prefabs/Actor/Cube1").Subscribe(asset => 
        {
            var cubeList = new Queue<GameObject>();
            for (var i = 0; i < 10; i++)
            {
                var cubeAsset = asset.Spawn();
                cubeAsset.name = "cube_" + i;
                cubeAsset.transform.position = new Vector3(URandom.Range(0, 10), 
                    URandom.Range(0, 10), URandom.Range(0, 10));
                cubeList.Enqueue(cubeAsset);

                Log.I("spawned " + cubeAsset.name);
            }

            Observable.Interval(TimeSpan.FromSeconds(3)).Subscribe(_ =>
            {
                if (cubeList.Count <= 6)
                {
                    var random = URandom.Range(0, 10 - cubeList.Count);
                    for (var i = 0; i < random; i++)
                    {
                        var cubeAsset = asset.Spawn();
                        cubeAsset.transform.position = new Vector3(URandom.Range(0, 10), 
                            URandom.Range(0, 10), URandom.Range(0, 10));
                        cubeList.Enqueue(cubeAsset);

                        Log.I("spawned " + cubeAsset.name);
                    }
                }
                else
                {
                    var random = URandom.Range(0, 5);
                    for (var i = 0; i < random; i++)
                    {
                        var cubeAsset = cubeList.Dequeue();
                        Log.I("despawned " + cubeAsset.name);
                        cubeAsset.Despawn();
                    }                  
                }
            });
        }).AddTo(disposables);

        AssetManager.Load<GameObject>("Prefabs/Actor/Sphere1").Subscribe(asset => 
        {
            var sphereList = new Queue<GameObject>();
            for (var i = 0; i < 8; i++)
            {
                var sphereAsset = asset.Spawn();
                sphereAsset.name = "sphere_" + i;
                sphereAsset.transform.position = new Vector3(URandom.Range(0, 10), 
                    URandom.Range(0, 10), URandom.Range(0, 10));
                sphereList.Enqueue(sphereAsset);

                Log.I("spawned " + sphereAsset.name);
            }

            Observable.Interval(TimeSpan.FromSeconds(5)).Subscribe(_ =>
            {
                if (sphereList.Count <= 4)
                {
                    var random = URandom.Range(0, 8 - sphereList.Count);
                    for (var i = 0; i < random; i++)
                    {
                        var sphereAsset = asset.Spawn();
                        sphereAsset.transform.position = new Vector3(URandom.Range(0, 10), 
                            URandom.Range(0, 10), URandom.Range(0, 10));
                        sphereList.Enqueue(sphereAsset);

                        Log.I("spawned " + sphereAsset.name);
                    }
                }
                else
                {
                    var random = URandom.Range(0, 5);
                    for (var i = 0; i < random; i++)
                    {
                        var sphereAsset = sphereList.Dequeue();
                        Log.I("despawned " + sphereAsset.name);
                        sphereAsset.Despawn();
                    }                  
                }
            });
        }).AddTo(disposables);
    }

    protected override void RegisterGameModule()
    {
    }
}
namespace ECS.Object.Data
{
    using ECS.Data;
    using ECS.Common;
    using System.Collections.Generic;
    using UnityEngine;

    public struct StateInfo
    {
        public string name;
        public int priority;
        public bool isLoop;
        public float process;

        public AnimationValueInfo[] startList;
        public AnimationValueInfo[] finishList;
    }

    public class ObjectStateData : IData, IPoolObject
    {
        public List<StateInfo> stateInfoList = new List<StateInfo>();
        public Animator animator;

        public bool IsInUse { get; set; }
        public void Clear()
        {
            stateInfoList.Clear();
            animator = null;
        }
    }
}
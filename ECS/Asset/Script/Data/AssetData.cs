namespace ECS.Data
{
    using UnityEngine;

    public class AssetData : MonoBehaviour, IData
    {
        [HideInInspector]
        public bool isSpawned;
        [HideInInspector]
        public uint unitId;
    }
}
namespace ECS.Config
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public struct AssetInfo
    {
        public string assetPath;
        public string fullPath;
        public string assetName;
        public string bundleName;
        public bool isFromBundle;
    }

    [CreateAssetMenu(menuName = AssetConstant.CONFIG_MENU_GROUP + "AssetConfig")]
    public class AssetConfig : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<AssetInfo> assetInfoList;
        public string[] indvAssetPathList;

        public Dictionary<string, AssetInfo> _assetInfoDict = new Dictionary<string, AssetInfo>();

        int FindAssetInfoIndex(string assetPath)
        {
            for (var i = 0; i < assetInfoList.Count; i++)
            {
                if (assetInfoList[i].assetPath == assetPath)
                {
                    return i;
                }
            }

            return -1;
        }

        public void Add(AssetInfo assetInfo)
        {
            var index = FindAssetInfoIndex(assetInfo.assetPath);
            if (index != -1)
            {
                assetInfoList[index] = assetInfo;
            }
            else
            {
                assetInfoList.Add(assetInfo);
            }
        }

        public void Remove(string assetPath)
        {
            var index = FindAssetInfoIndex(assetPath);
            if (index != -1)
            {
                assetInfoList.RemoveAt(index);
            }
        }

        public bool Contains(string assetPath)
        {
            return _assetInfoDict.ContainsKey(assetPath);
        }

        public AssetInfo GetAssetInfo(string assetPath)
        {
            AssetInfo assetInfo;
            _assetInfoDict.TryGetValue(assetPath, out assetInfo);
            return assetInfo;
        }

        public void Clear()
        {
            assetInfoList.Clear();
            _assetInfoDict?.Clear();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _assetInfoDict.Clear();
            foreach (var assetInfo in assetInfoList)
            {
                _assetInfoDict.Add(assetInfo.assetPath, assetInfo);
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }
    }
}
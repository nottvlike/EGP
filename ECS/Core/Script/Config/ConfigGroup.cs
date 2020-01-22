namespace ECS.Config
{
    using UnityEngine;
    using ECS;
    using ECS.Common;

    public struct ConfigGroupInfo
    {
        public string name;
        public ScriptableObject config;
    }

    [CreateAssetMenu(menuName = Constant.CONFIG_MENU_GROUP + "ConfigGroup")]
    public class ConfigGroup : ScriptableObject
    {
        public ConfigGroupInfo[] configGroupInfoList;

        public T Get<T>(string name = null) where T : ScriptableObject
        {
            var configName = string.IsNullOrEmpty(name) ? typeof(T).ToString() : name;
            for (var i = 0; i < configGroupInfoList.Length; i++)
            {
                var configGroupInfo = configGroupInfoList[i];
                if (configGroupInfo.name == configName)
                    return configGroupInfo.config as T;
            }

            Log.W("Failed to find config " + configName);
            return null;
        }
    }
}
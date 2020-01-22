namespace ECS.Config
{
    using ECS.Common;
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    public sealed class ConfigManager
    {
        Dictionary<string, ScriptableObject> _configDict = new Dictionary<string, ScriptableObject>();

        public T Get<T>(string name = null) where T : ScriptableObject
        {
            var configName = name != null ? name : typeof(T).ToString();
#if DEBUG
            if (!_configDict.ContainsKey(configName))
            {
                Log.E("failed to find config " + configName);
                return null;
            }
#endif
            return _configDict[configName] as T;
        }

        public void LoadConfig(ScriptableObject config, string name = null)
        {
            var configName = name != null ? name : config.GetType().ToString();
#if DEBUG
            if (_configDict.ContainsKey(configName))
            {
                Log.W("duplicate config " + configName + " will replac the old one!");
            }
#endif
            _configDict[configName] = config;
        }

        public void LoadConfigGroup(ConfigGroup configGroup)
        {
            foreach (var configGroupInfo in configGroup.configGroupInfoList)
            {
                LoadConfig(configGroupInfo.config, configGroupInfo.name);
            }
        }
    }
}
namespace ECS.Config
{
    using ECS.Common;
    using UnityEngine;
    using System.Collections.Generic;
    using System;

    public sealed class ConfigManager
    {
        Dictionary<Type, ScriptableObject> _configDict = new Dictionary<Type, ScriptableObject>();

        public T Get<T>() where T : ScriptableObject
        {
            var configType = typeof(T);
#if DEBUG
            if (!_configDict.ContainsKey(configType))
            {
                Log.E("failed to find config " + configType.ToString());
                return null;
            }
#endif
            return _configDict[configType] as T;
        }

        public void LoadConfig(ScriptableObject config)
        {
            var configType = config.GetType();
#if DEBUG
            if (_configDict.ContainsKey(configType))
            {
                Log.W("duplicate config " + configType.ToString() + " will replac the old one!");
            }
#endif
            _configDict[configType] = config;
        }

        public void LoadConfigGroup(ConfigGroup configGroup)
        {
            foreach (var config in configGroup.configList)
            {
                LoadConfig(config);
            }
        }
    }
}
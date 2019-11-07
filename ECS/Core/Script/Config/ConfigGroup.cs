namespace ECS.Config
{
    using UnityEngine;
    using ECS;
    using ECS.Common;

    [CreateAssetMenu(menuName = Constant.CONFIG_MENU_GROUP + "ConfigGroup")]
    public class ConfigGroup : ScriptableObject
    {
        public ScriptableObject[] configList;

        public T Get<T>() where T : ScriptableObject
        {
            for (var i = 0; i < configList.Length; i++)
            {
                var scriptableObject = configList[i];
                if (scriptableObject.GetType() == typeof(T))
                    return scriptableObject as T;
            }

            Log.W("Failed to find config " + typeof(T));
            return null;
        }
    }
}
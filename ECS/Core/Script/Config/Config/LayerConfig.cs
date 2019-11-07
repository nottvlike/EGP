namespace ECS.Config
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(menuName = Constant.CONFIG_MENU_GROUP + "LayerConfig")]
    public class LayerConfig : ScriptableObject
    {
        public List<string> tagList;

        public int TagToLayer(string tag)
        {
            return tagList.IndexOf(tag);
        }

        public string LayerToTag(int layer)
        {
            return tagList[layer];
        }
    }
}
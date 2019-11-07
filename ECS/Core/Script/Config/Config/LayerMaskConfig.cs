namespace ECS.Config
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(menuName = Constant.CONFIG_MENU_GROUP + "LayerMaskConfig")]
    public class LayerMaskConfig : ScriptableObject
    {
        public List<string> tagList;

        public int TagToLayer(string tag)
        {
            return 1 << tagList.IndexOf(tag);
        }

        public string LayerToTag(int layer)
        {
            var index = 0;
            while (layer > 0)
            {
                layer = layer >> 1;
                index++;
            }
            return tagList[index];
        }
    }
}
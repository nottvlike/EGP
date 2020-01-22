namespace ECS.Object.Config
{
    using UnityEngine;
    using System;

    [Serializable]
    public struct AttributeInfo
    {
        public string name;
        public string description;
    }

    [CreateAssetMenu(menuName = Constant.CONFIG_MENU_GROUP + "AttributeConfg")]
    public class AttributeConfig : ScriptableObject
    {
        public AttributeInfo[] attributeInfoList;
    }
}
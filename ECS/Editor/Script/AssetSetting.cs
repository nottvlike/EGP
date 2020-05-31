namespace AssetEditor
{
    using UnityEngine;
    using ECS.Config;
    using ECS;

    [CreateAssetMenu(menuName = Constant.CONFIG_MENU_GROUP + "AssetSetting")]
    public class AssetSetting : ScriptableObject
    {
        public AssetConfig assetConfig;
    }
}
namespace ECS
{
    using ECS.Common;

    [MergeConstant]
    internal class AssetConstant
    {
        public const string ASSET_CORE_UNIT_NAME = "AssetCore";

        public const string ASSET_UNIT_TYPE_NAME = "Asset";
        public const string ASSET_MODULE_GROUP_NAME = "Asset";

        public const string HTTP_ETAG_FLAG = "ETag";
        public const string HTTP_FILE_LENGTH_FLAG = "Content-Length";

        public const int HTTP_RESPONSE_CODE_OK = 200;

        public const int SIZE_KB = 1024;

        public const string MANIFEST_ETAG_KEY = "ManifestEtag";
        public const string MANIFEST_VERSION_KEY = "ManifestVersion";

        public const string MANIFEST_BUNDLE_NAME = "AssetBundles";

        public const string MANIFEST_NAME = "AssetBundleManifest";

        public const string ASSET_CONFIG_NAME = "AssetConfig.asset";

        public const string ASSET_CONFIG_BUNDLE_NAME = "bundle_assetconfig";
        public const string ASSET_CONFIG_ASSET_NAME = "assetconfig";

        public const string UNIT_TYPE_CONFIG_PATH = "Assets/EGP/Resources/Config/UnitTypeConfig.asset";

        public const string MODULE_GROUP_TYPE_CONFIG_PATH = "Assets/EGP/Resources/Config/ModuleGroupTypeConfig.asset";

        public const string CONFIG_MENU_GROUP = "Config/";

        public const string BUNDLE_RESOURCE_PATH_FLAG = "/BundleResources/";
        public const string RESOURCE_PATH_FLAG = "/Resources/";
        public const string EDITOR_PATH_FLAG = "/Editor/";

        public const string ASSETS_PATH_FLAG = "Assets";

        public const string INDV_PATH_FLAG = "_INDV";
        public const string PACK_PATH_FLAG = ".pack";

        public const string SLASH_WINDOWS = "\\";        
        public const string SLASH = "/";
        public const string UNDERLINE = "_";

        public const string EXTENSION_META = ".meta";

        public const string UNUSED_ASSET_FLAG = "_Unused";
    }
}
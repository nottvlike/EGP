namespace AssetEditor
{
    using ECS;
    using UnityEditor;
    using UnityEngine;

    public class BuildHelper
    {
        [MenuItem(AssetEditorConstant.BUILD_ASSETBUNDLE_MENU_NAME)]
        public static void BuildStandaloneAssetBundle()
        {
            AutoProcessor.RefreshAssetConfig();

            var outputPath = Application.dataPath.Replace(Constant.ASSETS_PATH_FLAG, string.Empty) 
                + Constant.MANIFEST_BUNDLE_NAME;
            BuildPipeline.BuildAssetBundles(outputPath, 
                BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
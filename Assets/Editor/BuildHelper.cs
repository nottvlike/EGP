namespace Game.Editor
{
    using Asset;
    using UnityEditor;
    using UnityEngine;

    public class BuildHelper
    {
        [MenuItem("Tools/Build Standalone Asset Bundle")]
        public static void BuildStandaloneAssetBundle()
        {
            var outputPath = Application.dataPath.Replace(AssetConstant.ASSETS_PATH_FLAG, string.Empty) 
                + AssetConstant.MANIFEST_BUNDLE_NAME;
            BuildPipeline.BuildAssetBundles(outputPath, 
                BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
        }
    }
}
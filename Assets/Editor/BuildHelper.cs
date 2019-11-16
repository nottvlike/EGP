namespace Game.Editor
{
    using Asset;
    using Asset.Builder;
    using AssetEditor;
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;

    [InitializeOnLoad]
    public class BuildHelper
    {
        static BuildHelper()
        {
            // AssetBundleBuilder.AddCondition(
            //     new ConditionInfo()
            //     {
            //         conditionList = new List<ICondition>()
            //         {
            //             new FileSizeCondition(0.5f),
            //             new FileExtensionCondition(new List<string>()
            //             {
            //                 ".png",
            //                 ".jpg",
            //                 ".ttf",
            //                 ".wav",
            //                 ".ogg",
            //                 ".mp3",
            //                 ".xmp",
            //                 ".fbx",
            //             }),
            //         }
            //     });

            AssetBundleBuilder.AddCondition(
                new ConditionInfo()
                {
                    conditionList = new List<ICondition>()
                    {
                        new DirectoryCondition(new List<string>()
                        {
                            "Prefabs/Actor",
                        }),
                    }
                });
        }

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
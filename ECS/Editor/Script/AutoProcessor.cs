namespace AssetEditor
{
    using UnityEngine;
    using UnityEditor;
    using System.IO;
    using System.Linq;
    using System.Collections.Generic;
    using ECS;
    using ECS.Helper;
    using ECS.Config;

    public class AutoProcessor : AssetPostprocessor
    {
        static AssetSetting _assetSetting;
        public static AssetConfig GetAssetConfig()
        {
            if (_assetSetting == null)
            {
                _assetSetting = AssetDatabase.LoadAssetAtPath<AssetSetting>(AssetEditorConstant.ASSET_SETTING_PATH);
            }

            return _assetSetting.assetConfig;
        }

        [MenuItem(AssetEditorConstant.REFRESH_ASSET_SETTING_MENU_NAME)]
        public static void RefreshAssetConfig()
        {
            var result = new List<string>();
            var rootPath = Application.dataPath.Replace(Constant.ASSETS_PATH_FLAG, string.Empty);
            SearchAllFiles(Application.dataPath, result, rootPath);

            foreach (var assetFullPath in result)
            {
                if (IsResourcePath(assetFullPath))
                {
                    AddToResourceConfig(assetFullPath);
                }
                else if (IsBundlePath(assetFullPath))
                {
                    AddToBundleConfig(assetFullPath);
                }
            }
        }

        static void SearchAllFiles(string path, List<string> resultList, string rootPath)
        {
            var allFileList = Directory.GetFiles(path);
            foreach (var file in allFileList)
            {
                var fileName = Path.GetFileName(file);
                if (!fileName.StartsWith(".") && Path.GetExtension(file) != Constant.EXTENSION_META)
                {
                    resultList.Add(file.Replace(rootPath, string.Empty));
                }
            }

            var allDirectoryList = Directory.GetDirectories(path);
            foreach (var directory in allDirectoryList)
            {
                SearchAllFiles(directory, resultList, rootPath);
            }
        }

        public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var assetPathList = importedAsset.Concat(movedAssets);
            foreach (string assetFullPath in assetPathList)
            {
                if (IsResourcePath(assetFullPath))
                {
                    AddToResourceConfig(assetFullPath);
                }
                else if (IsBundlePath(assetFullPath))
                {
                    AddToBundleConfig(assetFullPath);
                }
            }

            assetPathList = deletedAssets.Concat(movedFromAssetPaths);
            foreach (string assetFullPath in assetPathList)
            {
                if (IsResourcePath(assetFullPath))
                {
                    RemoveFromResourceConfig(assetFullPath);
                }
                else if (IsBundlePath(assetFullPath))
                {
                    RemoveFromBundleConfig(assetFullPath);
                }
            }
        }

        static bool IsBundlePath(string assetFullPath)
        {
            var filePath = Application.dataPath.Replace(Constant.ASSETS_PATH_FLAG, string.Empty);
            if (Directory.Exists(Path.Combine(filePath, assetFullPath)))
                return false;

            return AssetPath.IsBundlePath(assetFullPath);
        }

        static bool IsResourcePath(string assetFullPath)
        {
            var filePath = Application.dataPath.Replace(Constant.ASSETS_PATH_FLAG, string.Empty);
            if (Directory.Exists(Path.Combine(filePath, assetFullPath)))
                return false;

            return AssetPath.IsResourcePath(assetFullPath)
                && Path.GetFileName(assetFullPath) != Constant.ASSET_CONFIG_NAME;
        }

        public static void AddToResourceConfig(string assetFullPath)
        {
            var assetPath = AssetPath.GetAssetPathFromResourcePath(assetFullPath);

            var assetConfig = GetAssetConfig();
            assetConfig.Add(new AssetInfo()
            {
                assetPath = assetPath,
                fullPath = assetFullPath,
                isFromBundle = false
            }); ;

            EditorUtility.SetDirty(assetConfig);
        }

        static void RemoveFromResourceConfig(string assetFullPath)
        {
            var assetPath = AssetPath.GetAssetPathFromResourcePath(assetFullPath);

            var assetConfig = GetAssetConfig();
            assetConfig.Remove(assetPath);

            EditorUtility.SetDirty(assetConfig);
        }

        public static void AddToBundleConfig(string assetFullPath)
        {
            if (Path.GetFileName(assetFullPath) == Constant.ASSET_CONFIG_NAME)
            {
                ChangeAssetConfigBundleName(assetFullPath);
            }
            else
            {
                ChangeAssetBundleName(assetFullPath);

                var assetPath = AssetPath.GetAssetPathFromBundlePath(assetFullPath);
                var assetConfig = GetAssetConfig();
                assetConfig.Add(new AssetInfo()
                {
                    assetPath = assetPath,
                    assetName = AssetPath.GetAssetName(assetPath),
                    bundleName = AssetPath.GetBundleName(assetPath, assetFullPath, assetConfig.indvAssetPathList),
                    isFromBundle = true
                });

                EditorUtility.SetDirty(assetConfig);
            }
        }

        static void RemoveFromBundleConfig(string assetFullPath)
        {
            var assetPath = AssetPath.GetAssetPathFromBundlePath(assetFullPath);

            var assetConfig = GetAssetConfig();
            assetConfig.Remove(assetPath);

            EditorUtility.SetDirty(assetConfig);
        }

        static void ChangeAssetBundleName(string assetFullPath)
        {
            if (assetFullPath.Contains(Constant.EXTENSION_META))
                return;

            var assetSetting = AssetDatabase.LoadAssetAtPath<AssetSetting>(AssetEditorConstant.ASSET_SETTING_PATH);
            var assetPath = AssetPath.GetAssetPathFromBundlePath(assetFullPath);
            var assetBundleName = AssetPath.GetBundleName(assetPath, assetFullPath, assetSetting.assetConfig.indvAssetPathList);
            var assetName = AssetPath.GetAssetName(assetPath);

            var assetImporter = AssetImporter.GetAtPath(assetFullPath);
            if (assetImporter.name != assetName
                || assetImporter.assetBundleName != assetBundleName)
            {
                assetImporter.assetBundleName = assetBundleName;
                assetImporter.name = assetName;

                EditorUtility.SetDirty(assetImporter);
            }
        }

        static void ChangeAssetConfigBundleName(string assetFullPath)
        {
            var assetImporter = AssetImporter.GetAtPath(assetFullPath);
            if (assetImporter.name != Constant.ASSET_CONFIG_ASSET_NAME
                || assetImporter.assetBundleName != Constant.ASSET_CONFIG_BUNDLE_NAME)
            {
                assetImporter.assetBundleName = Constant.ASSET_CONFIG_BUNDLE_NAME;
                assetImporter.name = Constant.ASSET_CONFIG_ASSET_NAME;

                EditorUtility.SetDirty(assetImporter);
            }
        }
    }
}
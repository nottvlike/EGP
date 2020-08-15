namespace ECS.Helper
{
    using ECS.Common;
    using System.IO;
    using System.Text;
    using UnityEngine;
    using System;
    
    public class AssetPath
    {
        static StringBuilder _strBuilder = new StringBuilder();

        public static bool IsResourcePath(string assetPath)
        {
            return assetPath.StartsWith(AssetConstant.ASSETS_PATH_FLAG) && assetPath.Contains(AssetConstant.RESOURCE_PATH_FLAG)
                && !assetPath.Contains(AssetConstant.EDITOR_PATH_FLAG);
        }

        public static bool IsBundlePath(string assetPath)
        {
            return assetPath.StartsWith(AssetConstant.ASSETS_PATH_FLAG) && assetPath.Contains(AssetConstant.BUNDLE_RESOURCE_PATH_FLAG)
                && !assetPath.Contains(AssetConstant.EDITOR_PATH_FLAG);
        }

        public static string GetAssetPathFromResourcePath(string resourcePath)
        {
            if (!IsResourcePath(resourcePath))
            {
                Log.E("Asset path {0} is not in resource path!", resourcePath);
                return string.Empty;
            }

            _strBuilder.Clear();
            _strBuilder.Append(resourcePath);

            var endIndex = resourcePath.IndexOf(AssetConstant.RESOURCE_PATH_FLAG) + AssetConstant.RESOURCE_PATH_FLAG.Length;
            _strBuilder.Remove(0, endIndex);

            var extension = Path.GetExtension(resourcePath);
            if (!string.IsNullOrEmpty(extension))
            {
                _strBuilder.Replace(extension, string.Empty);
            }

            return _strBuilder.ToString();
        }

        public static string GetAssetPathFromBundlePath(string bundlePath)
        {
            if (!IsBundlePath(bundlePath))
            {
                Log.E("Asset path {0} is not in bundle path!", bundlePath);
                return string.Empty;
            }

            _strBuilder.Clear();
            _strBuilder.Append(bundlePath);

            var endIndex = bundlePath.IndexOf(AssetConstant.BUNDLE_RESOURCE_PATH_FLAG) + AssetConstant.BUNDLE_RESOURCE_PATH_FLAG.Length;
            _strBuilder.Remove(0, endIndex);

            var extension = Path.GetExtension(bundlePath);
            if (!string.IsNullOrEmpty(extension))
            {
                _strBuilder.Replace(extension, string.Empty);
            }

            return _strBuilder.ToString();
        }

        internal static string ConvertToBundleName(string str)
        {
            _strBuilder.Clear();
            return _strBuilder.Append(str).Replace(AssetConstant.SLASH_WINDOWS, AssetConstant.SLASH)
                        .Replace(AssetConstant.SLASH, AssetConstant.UNDERLINE)
                        .ToString().ToLower();
        }

        public static string GetBundleName(string assetPath, string assetFullPath, string[] indvAssetPathList)
        {
            bool isIndvPath = false;
            var indvAssetPath = string.Empty;
            foreach (var path in indvAssetPathList)
            {
                if (assetFullPath.Contains(path))
                {
                    indvAssetPath = path;
                    isIndvPath = true;
                    break;
                }
            }

            if (isIndvPath)
            {
                var nextSlashIndex = assetFullPath.IndexOf(AssetConstant.SLASH, indvAssetPath.Length + 1);
                if (nextSlashIndex == -1)
                {
                    return ConvertToBundleName(assetPath);
                }
                else
                {
                    return ConvertToBundleName(GetAssetPathFromBundlePath(assetFullPath.Remove(nextSlashIndex)));
                }
            }
            else
            {
                return ConvertToBundleName(assetPath);
            }
        }

        public static string GetAssetName(string assetPath)
        {
            return Path.GetFileNameWithoutExtension(assetPath);
        }

        public static Hash128 Version2Hash(int version)
        {
            return new Hash128(0, 0, 0, (UInt32)version);
        }
    }
}
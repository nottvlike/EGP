namespace ECS.Helper
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Networking;
    using System.Linq;

    internal static class UnityWebRequestExtension
    {
        public static IObservable<AssetBundle> LoadAssetBundle(this IObservable<UnityWebRequest> webRequest)
        {
            return webRequest.Select(req => 
            {
                return DownloadHandlerAssetBundle.GetContent(req);
            });
        }

        public static IObservable<UnityWebRequestAsyncOperation> SendAsObserable(this UnityWebRequest request)
        {
            if (request == null) return Observable.Empty<UnityWebRequestAsyncOperation>();
            return request.SendWebRequest().AsAsyncOperationObservable();
        }
    }
}
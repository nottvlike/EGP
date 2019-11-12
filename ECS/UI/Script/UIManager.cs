namespace ECS.UI
{
    using UniRx;
    using System;

    public class UIManager
    {
        static UIManagerInstance _instance => UIManagerInstance.Instance;

        public static void Init()
        {
            _instance.Init();
        }

        public static bool IsShowed(string assetPath)
        {
            return _instance.IsShowed(assetPath);
        }

        public static IObservable<Unit> Show(string assetPath, bool forceUpdateWhenShowed, params object[] args)
        {
            return _instance.LoadUIRoot().ContinueWith(_ => _instance.LoadUI(assetPath))
                .Finally(() => _instance.Show(assetPath, forceUpdateWhenShowed, args));
        }

        public static IObservable<Unit> Hide(string assetPath)
        {
            return _instance.Hide(assetPath);
        }
    }
}
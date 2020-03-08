namespace Helper
{
    using UnityEngine;
    using UnityEditor;
    using ECS.UI;
    using UniRx;

    public sealed class UIManagerHelper : MonoBehaviour
    {
        public void Show(string assetPath)
        {
            UIManager.Show(assetPath);
        }

        public void Hide(string assetPath)
        {
            UIManager.Hide(assetPath);
        }

        public void Exit()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif    
        }
    }
}
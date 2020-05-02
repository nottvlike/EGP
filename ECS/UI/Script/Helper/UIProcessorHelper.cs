namespace ECS.Helper
{
    using UnityEngine;
    using UnityEditor;
    using ECS.Module;

    public sealed class UIProcessorHelper : MonoBehaviour
    {
        public void Show(string assetPath)
        {
            UIProcessor.Show(assetPath);
        }

        public void Hide(string assetPath)
        {
            UIProcessor.Hide(assetPath);
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
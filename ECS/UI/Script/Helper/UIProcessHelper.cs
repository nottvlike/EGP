namespace ECS.Helper
{
    using UnityEngine;
    using UnityEditor;
    using ECS.Module;

    public sealed class UIProcessHelper : MonoBehaviour
    {
        public void Show(string assetPath)
        {
            UIProcess.Show(assetPath);
        }

        public void Hide(string assetPath)
        {
            UIProcess.Hide(assetPath);
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
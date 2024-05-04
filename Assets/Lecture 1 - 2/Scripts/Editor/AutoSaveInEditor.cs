using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RPS
{
    public class AutoSaveInEditor : MonoBehaviour
    {
        private static readonly float saveInterval = 60;
        private static double nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            EditorApplication.update += Update;
            EditorApplication.playModeStateChanged += SaveOnPlay;
        }

        private static void Update()
        {
            if (EditorApplication.timeSinceStartup >= nextSaveTime)
            {
                Save();
                nextSaveTime = EditorApplication.timeSinceStartup + saveInterval;
            }
        }

        private static void SaveOnPlay(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingEditMode)
                Save();
        }

        private static void Save()
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
            {
                Debug.Log("Auto-saving...");
                EditorSceneManager.SaveOpenScenes();
            }
            else
                Debug.Log("Scene is already saved.");
        }
    }
}
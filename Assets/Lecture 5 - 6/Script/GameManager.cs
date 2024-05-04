using UnityEngine;
using UnityEngine.SceneManagement;

namespace NGO_ToonTanks
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform[] _startTransform;
        private int _currentStartTransformIndex = 0;

        private void Start()
        {
            if (NetworkingManager.Singleton.IsServer)
            {
                NetworkingManager.Singleton.SceneManager.OnLoadComplete += SceneManager_OnLoadComplete;
                NetworkingManager.Singleton.OnServerStopped += Singleton_OnServerStopped;

                if (NetworkingManager.Singleton.IsHost)
                    SpawnNextPlayerObject(NetworkingManager.Singleton.LocalClientId);
            }
        }

        private void SceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
        {
            if (sceneName == GameConstant.SceneName_Gameplay)
                SpawnNextPlayerObject(clientId);
        }

        private void SpawnNextPlayerObject(ulong clientId)
        {
            if (_currentStartTransformIndex >= _startTransform.Length)
                _currentStartTransformIndex = 0;

            NetworkingManager.Singleton.SpawnPlayerObject(clientId, _startTransform[_currentStartTransformIndex].position, _startTransform[_currentStartTransformIndex].rotation);
            _currentStartTransformIndex++;
        }

        private void Singleton_OnServerStopped(bool obj)
        {
            if (NetworkingManager.Singleton != null && NetworkingManager.Singleton.SceneManager != null)
            {
                NetworkingManager.Singleton.SceneManager.OnLoadComplete -= SceneManager_OnLoadComplete;
                NetworkingManager.Singleton.OnServerStopped -= Singleton_OnServerStopped;
            }
        }
    }
}
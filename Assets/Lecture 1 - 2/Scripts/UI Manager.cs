using TMPro;
using UnityEngine;

namespace RPS
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _playerName;

        private NetworkingManager _networkManager;

        public static UIManager Singleton { get; private set; }

        private void Awake()
        {
            if (Singleton == null)
            {
                Singleton = this;
                transform.parent = null;
                DontDestroyOnLoad(gameObject);
            }
            else if (Singleton != this)
                Destroy(gameObject);
        }

        private void Start() => _networkManager = NetworkingManager.Singleton;

        public void OnStartServerClicked() => _networkManager.StartServer();

        public void OnStartHostClicked()
        {
            if (!string.IsNullOrEmpty(_playerName.text))
            {
                _networkManager.PlayerName = _playerName.text;
                _networkManager.StartHost();
            }
        }

        public void OnStartClientClicked()
        {
            if (!string.IsNullOrEmpty(_playerName.text))
            {
                _networkManager.PlayerName = _playerName.text;
                _networkManager.StartClient();
            }
        }
    }
}
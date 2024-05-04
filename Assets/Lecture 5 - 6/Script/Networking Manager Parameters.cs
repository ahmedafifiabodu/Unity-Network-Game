using Unity.Netcode;
using UnityEngine;

namespace NGO_ToonTanks
{
    public class NetworkingManagerParameters : MonoBehaviour
    {
        [SerializeField] internal NetworkObject _playerPrefab;
        [SerializeField] private PlayerData _playerData;

        internal PlayerData PlayerData => _playerData;

        public static NetworkingManagerParameters Singleton { get; private set; }

        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
        }
    }
}
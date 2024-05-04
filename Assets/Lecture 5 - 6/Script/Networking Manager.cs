using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NGO_ToonTanks
{
    public class NetworkingManager : NetworkManager
    {
        private Dictionary<ulong, NetworkingPlayer> _netowrkPlayers;

        public new static NetworkingManager Singleton { get; private set; }

        internal PlayerData PlayerData { get; private set; }

        internal Dictionary<ulong, NetworkingPlayer> NetworkPlayers { get => _netowrkPlayers; set => _netowrkPlayers = value; }

        internal NetworkingPlayer LocalPlayer => LocalClient.PlayerObject.GetComponent<NetworkingPlayer>();

        private void Awake()
        {
            if (Singleton == null)
                Singleton = this;
        }

        private void Start()
        {
            OnServerStarted += NetworkingManager_OnServerStarted;
            NetworkPlayers = new();
        }

        private void OnDestroy() => OnServerStarted -= NetworkingManager_OnServerStarted;

        private void NetworkingManager_OnServerStarted() => SceneManager.LoadScene(GameConstant.SceneName_Gameplay, LoadSceneMode.Single);

        #region Player Management

        public void SpawnPlayerObject(ulong clientId, Vector3 _startPosition, Quaternion _rotation)
        {
            if (!IsServer)
                return;

            NetworkObject _playerObject = Instantiate(NetworkingManagerParameters.Singleton._playerPrefab, _startPosition, _rotation);
            _playerObject.SpawnAsPlayerObject(clientId);
        }

        internal void UpdatePlayerData(PlayerData _playerData) => PlayerData = _playerData;

        internal void AddPlayer(NetworkingPlayer _player)
        {
            if (!NetworkPlayers.ContainsKey(_player.OwnerClientId))
                NetworkPlayers[_player.OwnerClientId] = _player;
        }

        internal NetworkingPlayer GetPlayerByID(ulong _playerID) => _netowrkPlayers[_playerID];

        #endregion Player Management
    }
}
using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace Mirror_Tanks
{
    public class NetworkingManager : NetworkManager
    {
        private new static NetworkingManager singleton;

        internal List<NetworkingPlayer> PlayersList { get; private set; }

        internal static NetworkingManager Singleton => singleton;
        internal bool IsHostActive { get; private set; }

        internal bool IsServer { get; private set; }
        internal bool IsClient { get; private set; }
        internal bool IsHost => IsServer && IsClient;

        internal Color Team1Color { get; set; }
        internal Color Team2Color { get; set; }

        #region Player Information

        internal int PlayerID { get; set; }
        internal string PlayerName { get; set; }
        internal float PlayerHealth { get; set; }
        internal float PlayerMaxHealth { get; set; }
        internal float PlayerScale { get; set; }
        internal float PlayerMovmementSpeed { get; set; }
        internal float PlayerCanonRotationSpeed { get; set; }
        internal int PlayerDamage { get; set; }
        internal float PlayerBulletSpeed { get; set; }

        internal float ReviveCooldown { get; set; }
        internal float ReviveTime { get; set; }

        #endregion Player Information

        #region Overrides Methods

        public override void Awake()
        {
            base.Awake();

            if (!singleton)
                singleton = this;

            PlayersList = new List<NetworkingPlayer>();
        }

        public override void Start() => base.Start();

        public override void OnStartServer()
        {
            base.OnStartServer();
            IsServer = true;

            GenerateTeamColors();
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            IsHostActive = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            IsClient = true;
        }

        #endregion Overrides Methods

        public void AddPlayer(NetworkingPlayer _player)
        {
            if (!PlayersList.Contains(_player))
                PlayersList.Add(_player);
        }

        public void RemovePlayer(NetworkingPlayer _player)
        {
            if (PlayersList.Contains(_player))
                PlayersList.Remove(_player);
        }

        private void GenerateTeamColors()
        {
            Team1Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            Team2Color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        internal NetworkingPlayer GetPlayerByNetworkingID(uint _netID) => PlayersList.Find(x => x.netId == _netID);
    }
}
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace NGO_ToonTanks
{
    public class NetworkingPlayer : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform _canvasPivot;

        [SerializeField] private TextMeshProUGUI _playerNameText;

        [Header("Health")]
        [SerializeField] private Slider _playerHP;

        private NetworkVariable<PlayerData> _playerData = new();
        private NetworkVariable<float> _playerHealth = new();
        private NetworkVariable<float> _playerSpeed = new();
        private NetworkVariable<float> _playerDamage = new();
        private NetworkVariable<float> _playerBulletSpeed = new();
        private int _playerMaxHealth;

        #region Properties

        internal NetworkVariable<PlayerData> PlayerData { get => _playerData; private set => _playerData = value; }
        private NetworkVariable<float> PlayerHealth { get => _playerHealth; set => _playerHealth = value; }
        internal NetworkVariable<float> PlayerSpeed { get => _playerSpeed; private set => _playerSpeed = value; }
        internal NetworkVariable<float> PlayerDamage { get => _playerDamage; private set => _playerDamage = value; }
        internal NetworkVariable<float> PlayerBulletSpeed { get => _playerBulletSpeed; private set => _playerBulletSpeed = value; }

        internal bool IsDead { get; private set; } = false;

        #endregion Properties

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            PlayerData.OnValueChanged += PlayerData_OnValueChanged;
            PlayerHealth.OnValueChanged += PlayerHealth_OnValueChanged;

            if (IsLocalPlayer)
            {
                UpdatePlayerDataServerRPC(NetworkingManager.Singleton.PlayerData);
                Debug.Log(NetworkingManager.Singleton.PlayerData.ToString());
            }
            else
                InitializePlayer();

            NetworkingManager.Singleton.AddPlayer(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            PlayerData.OnValueChanged -= PlayerData_OnValueChanged;
            PlayerHealth.OnValueChanged -= PlayerHealth_OnValueChanged;
        }

        #region Server Logic

        [ServerRpc] private void UpdatePlayerDataServerRPC(PlayerData _playerData) => PlayerData.Value = _playerData;

        #endregion Server Logic

        #region Client Logic

        internal void ApplyDamage(float _damage, ulong _bulletOwnerID)
        {
            if (PlayerHealth.Value == 0)
                return;

            PlayerHealth.Value -= _damage;
            PlayerHealth.Value = Mathf.Max(0, PlayerHealth.Value);

            if (PlayerHealth.Value == 0)
                KillClientRPC(_bulletOwnerID);
        }

        [ClientRpc]
        private void KillClientRPC(ulong playerNetID)
        {
            IsDead = true;

            NetworkingPlayer _player = NetworkingManager.Singleton.GetPlayerByID(playerNetID);

            if (_player)
                Debug.Log($"{_player.PlayerData.Value.PlayerName} killed {_playerData.Value.PlayerName}!");
        }

        #endregion Client Logic

        private void InitializePlayer()
        {
            _playerNameText.text = PlayerData.Value.PlayerName.ToString();
            PlayerHealth_OnValueChanged(0, PlayerHealth.Value);
        }

        private void PlayerData_OnValueChanged(PlayerData previousValue, PlayerData newValue)
        {
            if (IsServer)
            {
                if (newValue.PlayerType == PlayerType.Tank)
                {
                    PlayerHealth.Value = 200;
                    PlayerSpeed.Value = 5f;
                    PlayerDamage.Value = 5f;
                    PlayerBulletSpeed.Value = 5f;
                }
                else if (newValue.PlayerType == PlayerType.DPS)
                {
                    PlayerHealth.Value = 100;
                    PlayerSpeed.Value = 10f;
                    PlayerDamage.Value = 10f;
                    PlayerBulletSpeed.Value = 10f;
                }

                _playerMaxHealth = (int)PlayerHealth.Value;
                _playerHP.maxValue = _playerMaxHealth;
            }

            InitializePlayer();
        }

        private void PlayerHealth_OnValueChanged(float previousValue, float newValue) => _playerHP.value = newValue;

        private void Update() => _canvasPivot.LookAt(Camera.main.transform);
    }
}
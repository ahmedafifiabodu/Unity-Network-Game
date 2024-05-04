using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror_Tanks
{
    public class NetworkingPlayer : NetworkBehaviour
    {
        [Header("UI")]
        [SerializeField] private Transform _canvasPivot;

        [SerializeField] private TextMeshProUGUI _playerNameText;

        [Header("Health")]
        [SerializeField] private Slider _playerHP;

        [SyncVar(hook = nameof(OnIDUpdated))] private int _playerID;
        [SyncVar(hook = nameof(OnNameUpdated))] private string _playerName;
        [SyncVar(hook = nameof(OnHealthUpdated))] private float _playerHealth;
        [SyncVar(hook = nameof(OnMaxHealthUpdated))] private float _playerMaxHealth;
        [SyncVar(hook = nameof(OnScaleUpdated))] private float _playerScale;
        [SyncVar(hook = nameof(OnMovementSpeedUpdated))] private float _playerMovementSpeed;
        [SyncVar(hook = nameof(OnCanonRotationSpeedUpdated))] private float _playerCanonSpeed;
        [SyncVar(hook = nameof(OnDamageUpdated))] private int _playerDamage;
        [SyncVar(hook = nameof(OnBulletSpeedUpdated))] private float _playerBulletSpeed;
        [SyncVar(hook = nameof(OnDeathUpdate))] private bool _isDead = false;

        [SyncVar(hook = nameof(OnReviveCooldownUpdated))] private float _reviveCooldown;
        [SyncVar(hook = nameof(OnReviveTimeUpdated))] private float _reviveTime;

        internal int PlayerID
        { get => _playerID; private set => _playerID = value; }

        internal float PlayerHealth
        { get => _playerHealth; private set => _playerHealth = value; }

        internal float PlayerMaxHealth
        { get => _playerMaxHealth; private set => _playerMaxHealth = value; }

        internal float PlayerScale
        { get => _playerScale; private set => _playerScale = value; }

        internal float PlayerMovementSpeed
        { get => _playerMovementSpeed; private set => _playerMovementSpeed = value; }

        internal float PlayerCanonSpeed
        { get => _playerCanonSpeed; private set => _playerCanonSpeed = value; }

        internal int PlayerDamage
        { get => _playerDamage; private set => _playerDamage = value; }

        internal float PlayerBulletSpeed
        { get => _playerBulletSpeed; private set => _playerBulletSpeed = value; }

        internal bool IsDead
        { get => _isDead; private set => _isDead = value; }

        internal float ReviveCooldown
        { get => _reviveCooldown; private set => _reviveCooldown = value; }

        internal float ReviveTime
        { get => _reviveTime; private set => _reviveTime = value; }

        internal bool IsBeingRevived { get; set; }

        private float deathTime;

        private PlayerMovement playerMovement;
        private PlayerShooting playerShooting;

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();

            CMDSetPlayerID(NetworkingManager.Singleton.PlayerID);
            CMDSetPlayerName(NetworkingManager.Singleton.PlayerName);
            CMDSetPlayerHealth(NetworkingManager.Singleton.PlayerHealth);
            CMDSetPlayerMaxHealth(NetworkingManager.Singleton.PlayerMaxHealth);
            CMDSetPlayerScale(NetworkingManager.Singleton.PlayerScale);
            CMDSetPlayerMovementSpeed(NetworkingManager.Singleton.PlayerMovmementSpeed);
            CMDSetPlayerCanonSpeed(NetworkingManager.Singleton.PlayerCanonRotationSpeed);
            CMDSetPlayerDamage(NetworkingManager.Singleton.PlayerDamage);
            CMDSetPlayerBulletSpeed(NetworkingManager.Singleton.PlayerBulletSpeed);

            CMDSetReviveCooldown(NetworkingManager.Singleton.ReviveCooldown);
            CMDSetReviveTime(NetworkingManager.Singleton.ReviveTime);

            CMDAssignRandomColor();
        }

        private void Awake()
        {
            playerMovement = GetComponent<PlayerMovement>();
            playerShooting = GetComponent<PlayerShooting>();
            playerRenderer = GetComponent<Renderer>();
        }

        private void Start() => NetworkingManager.Singleton.AddPlayer(this);

        private void Update()
        {
            _canvasPivot.LookAt(Camera.main.transform);

            _playerHP.value = PlayerHealth;
        }

        private void OnDestroy() => NetworkingManager.Singleton.RemovePlayer(this);

        #region Player Color

        private Renderer playerRenderer;

        [SyncVar(hook = nameof(OnColorChanged))] private Color playerColor;

        [Command]
        private void CMDAssignRandomColor()
        {
            Color teamColor;

            if (PlayerID == 1)
            {
                teamColor = NetworkingManager.Singleton.Team1Color;
                playerColor = teamColor;
            }
            else if (PlayerID == 2)
            {
                teamColor = NetworkingManager.Singleton.Team2Color;
                playerColor = teamColor;
            }
        }

        private void OnColorChanged(Color _, Color newColor) => playerRenderer.material.color = newColor;

        #endregion Player Color

        #region RPC

        //[Command] - Calls an RPC from the client to the server
        //[ClientRpc] - Calls an RPC from the server to the all client
        //[TargetRpc] - Calls an RPC from the server to a specific client
        //[Client] - Restricts calling a local method on a client
        //[Server] - Restricts calling a local method on a server

        #region Commands

        [Command] private void CMDSetPlayerName(string _name) => _playerName = _name;

        [Command] private void CMDSetPlayerID(int _playerID) => PlayerID = _playerID;

        [Command] private void CMDSetPlayerHealth(float _playerHealth) => PlayerHealth = _playerHealth;

        [Command] private void CMDSetPlayerMaxHealth(float _playerMaxHealth) => PlayerMaxHealth = _playerMaxHealth;

        [Command] private void CMDSetPlayerScale(float _playerscale) => transform.localScale = new Vector3(_playerscale, _playerscale, _playerscale);

        [Command] private void CMDSetPlayerMovementSpeed(float _playerMovementSpeed) => PlayerMovementSpeed = _playerMovementSpeed;

        [Command] private void CMDSetPlayerCanonSpeed(float _playerCanonSpeed) => PlayerCanonSpeed = _playerCanonSpeed;

        [Command] private void CMDSetPlayerDamage(int _playerDamage) => PlayerDamage = _playerDamage;

        [Command] private void CMDSetPlayerBulletSpeed(float _playerBulletSpeed) => PlayerBulletSpeed = _playerBulletSpeed;

        [Command] private void CMDSetReviveCooldown(float _playerReviveCooldown) => ReviveCooldown = _playerReviveCooldown;

        [Command] private void CMDSetReviveTime(float _playerReviveTime) => ReviveTime = _playerReviveTime;

        #endregion Commands

        #region Norifications

        [ClientRpc] private void RpcDisplayKillerNotification(string killed, string killer) => UIManager.Singleton.DisplayNotification(killed, killer);

        [ClientRpc] internal void RpcDisplayWinnerNotification() => UIManager.Singleton.CheckWinningTeam();

        internal void UpdateReviveNotification(float cooldown)
        {
            float remainingTime = deathTime + cooldown - Time.time;

            if (remainingTime > 0)
                UIManager.Singleton.DisplayReviveNotification(_playerName, Mathf.Max(0, (int)remainingTime));
        }

        #endregion Norifications

        #region Revive

        internal bool CanBeRevived(float cooldown)
        {
            float remainingTime = deathTime + cooldown - Time.time;
            return remainingTime <= 0;
        }

        internal void RpcRevivePlayer()
        {
            IsDead = false;
            IsBeingRevived = false;
            PlayerHealth = PlayerMaxHealth;
            OnHealthUpdated(0, PlayerHealth);
            gameObject.SetActive(true);
        }

        #endregion Revive

        [ClientRpc]
        private void Dead(uint _netID)
        {
            IsDead = true;
            deathTime = Time.time;

            string killerName = NetworkingManager.Singleton.GetPlayerByNetworkingID(_netID)._playerName;

            if (isServer)
            {
                RpcDisplayKillerNotification(_playerNameText.text, killerName);
                RpcDisplayWinnerNotification();
            }

            if (isClient)
            {
                UIManager.Singleton.DisplayNotification(_playerNameText.text, killerName);
                UIManager.Singleton.CheckWinningTeam();
            }

            gameObject.SetActive(false);
        }

        [Server]
        internal void ApplyDamage(float _damage, uint _netID)
        {
            PlayerHealth -= _damage;

            if (PlayerHealth <= 0)
            {
                PlayerHealth = 0;
                Dead(_netID);
            }
        }

        #endregion RPC

        #region Hooks

        private void OnIDUpdated(int _, int newID) => PlayerID = newID;

        private void OnNameUpdated(string _, string newName) => _playerNameText.SetText(newName);

        private void OnHealthUpdated(float _, float newHealth)
        {
            PlayerHealth = newHealth;
            _playerHP.value = newHealth;
        }

        private void OnMaxHealthUpdated(float _, float newMaxHealth)
        {
            PlayerMaxHealth = newMaxHealth;
            _playerHP.maxValue = newMaxHealth;
        }

        private void OnScaleUpdated(float _, float newScale) => PlayerScale = newScale;

        private void OnMovementSpeedUpdated(float _, float newSpeed) => PlayerMovementSpeed = newSpeed;

        private void OnCanonRotationSpeedUpdated(float _, float newSpeed) => PlayerCanonSpeed = newSpeed;

        private void OnDamageUpdated(int _, int newDamage) => PlayerDamage = newDamage;

        private void OnBulletSpeedUpdated(float _, float newSpeed) => PlayerBulletSpeed = newSpeed;

        private void OnDeathUpdate(bool _, bool newDead) => IsDead = newDead;

        private void OnReviveCooldownUpdated(float _, float newCooldown) => ReviveCooldown = newCooldown;

        private void OnReviveTimeUpdated(float _, float newTime) => ReviveTime = newTime;

        #endregion Hooks
    }
}
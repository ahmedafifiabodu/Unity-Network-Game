using System.Collections;
using TMPro;
using UnityEngine;

namespace NGO_ToonTanks
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private TMP_InputField if_playerName;
        [SerializeField] private GameObject _teamPanel;
        [SerializeField] private GameObject _playerType;

        private NetworkingManager _networkManager;

        private PlayerType playerType;
        private Team team;

        private bool isHost = false;
        private bool isClient = false;

        private void Start() => _networkManager = NetworkingManager.Singleton;

        public void OnStartServerClicked() => _networkManager.StartServer();

        public void OnStartHostClicked()
        {
            if (!string.IsNullOrEmpty(if_playerName.text))
            {
                _teamPanel.SetActive(true);
                isHost = true;
            }
        }

        public void OnStartClientClicked()
        {
            if (!string.IsNullOrEmpty(if_playerName.text))
            {
                _teamPanel.SetActive(true);
                isClient = true;
            }
        }

        public void OnTeam1Clicked()
        {
            //_networkManager.PlayerName = if_playerName.text;
            // _networkManager.PlayerID = 1;

            team = Team.Team1;

            _teamPanel.SetActive(false);
            _playerType.SetActive(true);
        }

        public void OnTeam2Clicked()
        {
            //_networkManager.PlayerName = if_playerName.text;
            //_networkManager.PlayerID = 2;

            team = Team.Team2;

            _teamPanel.SetActive(false);
            _playerType.SetActive(true);
        }

        public void OnTankClicked()
        {
            //_networkManager.PlayerHealth = 200f;
            //_networkManager.PlayerMaxHealth = _networkManager.PlayerHealth;
            //_networkManager.PlayerScale = 1.5f;
            //_networkManager.PlayerMovmementSpeed = 5f;
            //_networkManager.PlayerCanonRotationSpeed = 2f;
            //_networkManager.PlayerDamage = 5;
            //_networkManager.PlayerBulletSpeed = 10f;

            //_networkManager.ReviveCooldown = 10f;
            //_networkManager.ReviveTime = 5f;

            playerType = PlayerType.Tank;

            StartCoroutine(StartHostOrClient());
        }

        public void OnDPSClicked()
        {
            //_networkManager.PlayerHealth = 100f;
            //_networkManager.PlayerMaxHealth = _networkManager.PlayerHealth;
            //_networkManager.PlayerScale = 0.7f;
            //_networkManager.PlayerMovmementSpeed = 10f;
            //_networkManager.PlayerCanonRotationSpeed = 5f;
            //_networkManager.PlayerDamage = 10;
            //_networkManager.PlayerBulletSpeed = 20f;

            //_networkManager.ReviveCooldown = 5f;
            //_networkManager.ReviveTime = 10f;

            playerType = PlayerType.DPS;

            StartCoroutine(StartHostOrClient());
        }

        private IEnumerator StartHostOrClient()
        {
            yield return new WaitForSeconds(1f);

            PlayerData playerData = new()
            {
                PlayerName = if_playerName.text,
                PlayerTeam = team,
                PlayerType = playerType
            };

            _networkManager.UpdatePlayerData(playerData);

            if (isHost)
                _networkManager.StartHost();
            else if (isClient)
                _networkManager.StartClient();
            else
                _networkManager.StartServer();
        }
    }
}
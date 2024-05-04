using TMPro;
using UnityEngine;

namespace Mirror_Tanks
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerNotification;
        [SerializeField] private TextMeshProUGUI _reviveNotification;
        [SerializeField] private TextMeshProUGUI _teamNotification;

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

            _playerNotification.gameObject.SetActive(false);
        }

        private void Start() => _networkManager = NetworkingManager.Singleton;

        public void OnStartServerClicked() => _networkManager.StartServer();

        #region Player Notification

        internal void DisplayNotification(string _playerKilled, string _playerKiller)
        {
            _playerNotification.gameObject.SetActive(true);
            _playerNotification.SetText($"{_playerKiller} killed {_playerKilled}");

            CancelInvoke(nameof(HideNotification));
            Invoke(nameof(HideNotification), 3f);
        }

        private void HideNotification() => _playerNotification.gameObject.SetActive(false);

        #endregion Player Notification

        #region Team Notification

        internal void CheckWinningTeam()
        {
            int team1AliveCount = 0;
            int team2AliveCount = 0;

            foreach (var player in _networkManager.PlayersList)
            {
                if (player.PlayerID == 1 && !player.IsDead)
                    team1AliveCount++;
                else if (player.PlayerID == 2 && !player.IsDead)
                    team2AliveCount++;
            }

            if (team1AliveCount == 0 && team2AliveCount > 0)
                DisplayWinningTeam(2);
            else if (team2AliveCount == 0 && team1AliveCount > 0)
                DisplayWinningTeam(1);
        }

        private void DisplayWinningTeam(int winningTeam)
        {
            _teamNotification.transform.parent.gameObject.SetActive(true);
            _teamNotification.SetText($"Team {winningTeam} are the winners!");

            Time.timeScale = 0;
        }

        #endregion Team Notification

        #region Revive Notification

        internal void DisplayReviveNotification(string _player, int _seconds)
        {
            _reviveNotification.gameObject.SetActive(true);
            _reviveNotification.SetText($"{_player} will be revived in {_seconds} seconds");

            CancelInvoke(nameof(HideReviveNotification));
            Invoke(nameof(HideReviveNotification), 3f);
        }

        private void HideReviveNotification() => _reviveNotification.gameObject.SetActive(false);

        #endregion Revive Notification
    }
}
using TMPro;
using UnityEngine;

namespace RPS
{
    public enum RPSMove
    {
        None = 0,
        Rock = 1,
        Paper = 2,
        Scissors = 3
    }

    public enum RPSResult
    {
        Draw = 0,
        Win = 1,
        Lose = 2
    }

    public class GameplayUI : MonoBehaviour
    {
        [Header("Player 1")]
        [SerializeField] private TextMeshProUGUI _player1Name;

        [SerializeField] private TextMeshProUGUI _player1Score;

        [Header("Player 2")]
        [SerializeField] private TextMeshProUGUI _player2Name;

        [SerializeField] private TextMeshProUGUI _player2Score;

        [Header("Win Screen")]
        [SerializeField] private TextMeshProUGUI _winScreen;

        [Header("Canvas Group")]
        [SerializeField] private CanvasGroup _canvasGroup;

        public static GameplayUI Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);

            SetText();
        }

        private void SetText()
        {
            _player1Name.SetText("");
            _player1Score.SetText("0");

            _player2Name.SetText("");
            _player2Score.SetText("0");
        }

        internal void UpdateName(bool _isLocalPlayer, string _playerName)
        {
            if (_isLocalPlayer)
                _player1Name.SetText(_playerName);
            else
                _player2Name.SetText(_playerName);
        }

        internal void UpdateScore(bool _isLocalPlayer, int _playerScore)
        {
            if (_isLocalPlayer)
                _player1Score.SetText(_playerScore.ToString());
            else
                _player2Score.SetText(_playerScore.ToString());
        }

        internal void SetWinScreen(string _result) => _winScreen.SetText(_result);

        internal void SetWinScreenColor(Color _color) => _winScreen.color = _color;

        internal void CanvasGroupInteractable(bool isInteractable) => _canvasGroup.interactable = isInteractable;

        public void OnActionTaken(int _move)
        {
            CanvasGroupInteractable(false);

            RPSMove _playermove = (RPSMove)_move;
            NetworkingManager.Singleton.LocalPlayer.CMDUpdatePlayerMove(_playermove);

            Debug.Log($"Player move: {_playermove}");
        }
    }
}
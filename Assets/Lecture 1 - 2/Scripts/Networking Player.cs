using Mirror;

namespace RPS
{
    public class NetworkingPlayer : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnPlayerNameUpdated))] private string _playerName;
        [SyncVar(hook = nameof(OnPlayerScore))] private int _playerScore;
        [SyncVar(hook = nameof(OnActionUpdated))] private RPSMove _playerMove;

        internal RPSMove PlayerMove { get => _playerMove; }

        internal int PlayerSocre { get => _playerScore; }

        private void Start() => NetworkingManager.Singleton.AddPlayer(this);

        public override void OnStartLocalPlayer()
        {
            base.OnStartLocalPlayer();
            CMDUpdatePlayerName(NetworkingManager.Singleton.PlayerName);
        }

        #region RPC

        //[Command] - Calls an RPC from the client to the server
        //[ClientRpc] - Calls an RPC from the server to the all client
        //[TargetRpc] - Calls an RPC from the server to a specific client
        //[Client] - Restricts calling a local method on a client
        //[Server] - Restricts calling a local method on a server

        [Command]
        private void CMDUpdatePlayerName(string _playerName) => this._playerName = _playerName;

        [Command]
        internal void CMDUpdatePlayerMove(RPSMove _move)
        {
            _playerMove = _move;
            NetworkingManager.Singleton.CheckToCalucateResult();
        }

        [TargetRpc]
        internal void TargetSetPlayersResult(RPSResult _rpsResult)
        {
            UnityEngine.Debug.Log($"Player {_playerName} result: {_rpsResult}");

            GameplayUI.Instance.CanvasGroupInteractable(true);

            if (_rpsResult == RPSResult.Win)
                GameplayUI.Instance.SetWinScreenColor(UnityEngine.Color.green);
            else if (_rpsResult == RPSResult.Lose)
                GameplayUI.Instance.SetWinScreenColor(UnityEngine.Color.red);
            else
                GameplayUI.Instance.SetWinScreenColor(UnityEngine.Color.grey);

            GameplayUI.Instance.SetWinScreen($"Player {_playerName} result: {_rpsResult}");
        }

        [Server]
        internal void UpdatePlayerScore(int _score) => _playerScore = _score;

        [TargetRpc]
        internal void ResetPlayerMoveAndUI()
        {
            _playerMove = RPSMove.None;
            GameplayUI.Instance.SetWinScreen("");
        }

        #endregion RPC

        #region Hooks

        private void OnPlayerNameUpdated(string _, string _newValue)
        {
            _playerName = _newValue;
            GameplayUI.Instance.UpdateName(isLocalPlayer, _playerName);
        }

        private void OnPlayerScore(int _, int _newValue)
        {
            _playerScore = _newValue;
            GameplayUI.Instance.UpdateScore(isLocalPlayer, _playerScore);
        }

        private void OnActionUpdated(RPSMove _, RPSMove _newValue) => _playerMove = _newValue;

        #endregion Hooks
    }
}
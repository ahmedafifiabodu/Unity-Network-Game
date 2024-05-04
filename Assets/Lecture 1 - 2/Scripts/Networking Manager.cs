using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPS
{
    public class NetworkingManager : NetworkManager
    {
        private new static NetworkingManager singleton;

        private List<NetworkingPlayer> _playersList;

        internal static NetworkingManager Singleton => singleton;
        internal bool IsServer { get; private set; }
        internal bool IsClient { get; private set; }
        internal bool IsHost => IsServer && IsClient;
        internal string PlayerName { get; set; }

        internal NetworkingPlayer LocalPlayer => _playersList.Find(x => x.isLocalPlayer);
        internal NetworkingPlayer OtherPlayer => _playersList.Find(x => !x.isLocalPlayer);

        #region Overrides Methods

        public override void Awake()
        {
            base.Awake();

            if (!singleton)
                singleton = this;

            _playersList = new List<NetworkingPlayer>();
        }

        public override void Start() => base.Start();

        public override void OnStartServer()
        {
            base.OnStartServer();
            IsServer = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            IsClient = true;
        }

        #endregion Overrides Methods

        public void AddPlayer(NetworkingPlayer _player)
        {
            if (!_playersList.Contains(_player))
                _playersList.Add(_player);
        }

        public void RemovePlayer(NetworkingPlayer _player)
        {
            if (_playersList.Contains(_player))
                _playersList.Remove(_player);
        }

        private bool DidAllPlayerMakeMove() => _playersList.TrueForAll(x => x.PlayerMove != RPSMove.None);

        internal void CheckToCalucateResult()
        {
            if (DidAllPlayerMakeMove())
                StartCoroutine(PlayersResult());
        }

        private IEnumerator PlayersResult()
        {
            yield return new WaitForSeconds(2f);

            RPSMove _player1Move = _playersList[0].PlayerMove;
            RPSMove _player2Move = _playersList[1].PlayerMove;

            RPSResult _player1Result;
            RPSResult _player2Result;

            if (_player1Move == _player2Move)
                _player1Result = _player2Result = RPSResult.Draw;
            else
            {
                /*switch (_player1Move)
                {
                    case RPSMove.Rock:
                        Debug.Log(_player2Move == RPSMove.Scissors ? "Player 1 wins" : "Player 2 wins");
                        break;

                    case RPSMove.Paper:
                        Debug.Log(_player2Move == RPSMove.Rock ? "Player 1 wins" : "Player 2 wins");
                        break;

                    case RPSMove.Scissors:
                        Debug.Log(_player2Move == RPSMove.Paper ? "Player 1 wins" : "Player 2 wins");
                        break;
                }*/

                _player1Result = _player1Move switch
                {
                    RPSMove.Rock => _player2Move == RPSMove.Scissors ? RPSResult.Win : RPSResult.Lose,
                    RPSMove.Paper => _player2Move == RPSMove.Rock ? RPSResult.Win : RPSResult.Lose,
                    RPSMove.Scissors => _player2Move == RPSMove.Paper ? RPSResult.Win : RPSResult.Lose,
                    _ => RPSResult.Lose
                };

                _player2Result = _player1Result == RPSResult.Win ? RPSResult.Lose : RPSResult.Win;

                int _player1Score = _playersList[0].PlayerSocre;
                int _player2Score = _playersList[1].PlayerSocre;

                if (_player1Result == RPSResult.Win)
                {
                    _player1Score++;
                    _player2Score--;
                }
                else if (_player1Result == RPSResult.Lose)
                {
                    _player2Score++;
                    _player1Score--;
                }

                _player1Score = Mathf.Max(0, _player1Score);
                _player2Score = Mathf.Max(0, _player2Score);

                _playersList[0].UpdatePlayerScore(_player1Score);
                _playersList[1].UpdatePlayerScore(_player2Score);
            }

            _playersList[0].TargetSetPlayersResult(_player1Result);
            _playersList[1].TargetSetPlayersResult(_player2Result);

            yield return new WaitForSeconds(2f);

            _playersList[0].ResetPlayerMoveAndUI();
            _playersList[1].ResetPlayerMoveAndUI();
        }
    }
}
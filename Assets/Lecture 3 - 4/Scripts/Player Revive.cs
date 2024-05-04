using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Mirror_Tanks
{
    public class PlayerRevive : MonoBehaviour
    {
        [SerializeField] private Slider _playerReviveSilder;

        private NetworkingPlayer networkingPlayer;
        private NetworkingPlayer deadPlayer;

        private void Start() => networkingPlayer = GetComponent<NetworkingPlayer>();

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameConstant.REVIVE_TAG))
            {
                deadPlayer = NetworkingManager.Singleton.PlayersList
                    .FirstOrDefault(p => p.IsDead && !p.IsBeingRevived && p.PlayerID == networkingPlayer.PlayerID);

                if (deadPlayer == null)
                    return;

                if (networkingPlayer.isLocalPlayer)
                    deadPlayer.UpdateReviveNotification(deadPlayer.ReviveCooldown);

                if (!deadPlayer.CanBeRevived(deadPlayer.ReviveCooldown))
                    return;

                deadPlayer.IsBeingRevived = true;
                _playerReviveSilder.gameObject.SetActive(true);
                StartCoroutine(ReviveProcess(deadPlayer));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(GameConstant.REVIVE_TAG))
            {
                if (deadPlayer != null)
                    deadPlayer.IsBeingRevived = false;

                _playerReviveSilder.value = 0;
                _playerReviveSilder.gameObject.SetActive(false);
                StopAllCoroutines();
            }
        }

        private IEnumerator ReviveProcess(NetworkingPlayer deadPlayer)
        {
            float reviveTime = 0;

            while (reviveTime < deadPlayer.ReviveTime)
            {
                _playerReviveSilder.value = reviveTime / deadPlayer.ReviveTime;
                reviveTime += Time.deltaTime;
                yield return null;
            }

            ReviveFirstDeadPlayer(deadPlayer);
        }

        private void ReviveFirstDeadPlayer(NetworkingPlayer deadPlayer)
        {
            if (deadPlayer != null)
                deadPlayer.RpcRevivePlayer();

            _playerReviveSilder.gameObject.SetActive(false);
        }
    }
}
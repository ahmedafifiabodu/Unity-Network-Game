using UnityEngine;

namespace NGO_ToonTanks
{
    public class Bullet : MonoBehaviour
    {
        private Rigidbody rb;

        private float _damage;
        private float _bulletSpeed;

        private ulong _playerNetID;

        private Team _networkingPlayerTeam;

        private void Awake() => rb = GetComponent<Rigidbody>();

        private void Start() => Destroy(gameObject, 5);

        internal void Init(ulong _ownerClientID, float _damage, float _bulletSpeed, Team _networkingPlayerTeam)
        {
            _playerNetID = _ownerClientID;
            this._damage = _damage;
            this._bulletSpeed = _bulletSpeed;
            this._networkingPlayerTeam = _networkingPlayerTeam;

            rb.velocity = transform.forward * this._bulletSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out NetworkingPlayer _player) && other.CompareTag(GameConstant.PlayerTag))
            {
                if (NetworkingManager.Singleton.IsServer)
                {
                    if (_player.PlayerData.Value.PlayerTeam != _networkingPlayerTeam)
                    {
                        _player.ApplyDamage(_damage, _playerNetID);
                        Destroy(gameObject);
                    }
                }
                else if (_player.PlayerData.Value.PlayerTeam != _networkingPlayerTeam)
                    Destroy(gameObject);
                else if (_player.PlayerData.Value.PlayerTeam != _networkingPlayerTeam)
                {
                }
            }
            else if (other.CompareTag(GameConstant.ReviveTag))
            {
            }
            else
                Destroy(gameObject);
        }
    }
}
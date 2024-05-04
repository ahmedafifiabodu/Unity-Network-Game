using UnityEngine;

namespace Mirror_Tanks
{
    public class Bullet : MonoBehaviour
    {
        private Rigidbody rb;

        private int _damage;

        private int _playerID;
        private uint _playerNetID;

        private void Awake() => rb = GetComponent<Rigidbody>();

        private void Start() => Destroy(gameObject, 5);

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out NetworkingPlayer _player) && other.CompareTag(GameConstant.PLAYER_TAG))
            {
                if (NetworkingManager.Singleton.IsServer)
                {
                    if (_player.PlayerID != _playerID)
                    {
                        _player.ApplyDamage(_damage, _playerNetID);
                        Destroy(gameObject);
                    }
                }
                else if (_player.PlayerID != _playerID)
                    Destroy(gameObject);
                else if (_player.PlayerID == _playerID)
                {
                }
            }
            else if (other.CompareTag(GameConstant.REVIVE_TAG))
            {
            }
            else
                Destroy(gameObject);
        }

        internal void SetPlayerId(uint _id) => _playerNetID = _id;

        internal void SetPlayerId(int _id) => _playerID = _id;

        internal void SetPlayerBulletDamage(int _damage) => this._damage = _damage;

        internal void SetPlayerBulletSpeed(float _speed) => rb.velocity = transform.forward * _speed;
    }
}
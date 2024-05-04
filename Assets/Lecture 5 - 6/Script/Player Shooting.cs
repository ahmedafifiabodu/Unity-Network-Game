using Unity.Netcode;
using UnityEngine;

namespace NGO_ToonTanks
{
    public class PlayerShooting : MonoBehaviour
    {
        [Header("Shooting")]
        [SerializeField] private Bullet _bulletPrefab;

        [SerializeField] private Transform _bulletLocation;
        [SerializeField] private Transform _bulletPivoit;

        private NetworkingPlayer _player;

        private void Start() => _player = GetComponent<NetworkingPlayer>();

        [ServerRpc]
        private void ShootServerRPC()
        {
            Debug.Log("ShootServerRPC");
            Bullet bullet = Instantiate(_bulletPrefab, _bulletLocation.position, _bulletLocation.rotation);
            bullet.Init(_player.OwnerClientId, _player.PlayerDamage.Value, _player.PlayerBulletSpeed.Value, _player.PlayerData.Value.PlayerTeam);
            ShootClientRPC(_player.OwnerClientId, _bulletLocation.position, _bulletLocation.rotation, _player.PlayerDamage.Value, _player.PlayerBulletSpeed.Value, _player.PlayerData.Value.PlayerTeam);
        }

        [ClientRpc]
        private void ShootClientRPC(ulong ownerId, Vector3 position, Quaternion rotation, float damage, float bulletSpeed, Team playerTeam)
        {
            if (!NetworkingManager.Singleton.IsHost)
            {
                Debug.Log("ShootClientRPC");
                Bullet bullet = Instantiate(_bulletPrefab, position, rotation);
                bullet.Init(ownerId, damage, bulletSpeed, playerTeam);
            }
        }

        private void Update()
        {
            if (_player.IsLocalPlayer && !_player.IsDead)
            {
                if (Input.GetMouseButtonDown(0))
                    ShootServerRPC();
            }
        }
    }
}
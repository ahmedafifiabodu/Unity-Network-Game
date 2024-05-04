using Mirror;
using UnityEngine;

namespace Mirror_Tanks
{
    public class PlayerShooting : NetworkBehaviour
    {
        [Header("Shooting")]
        [SerializeField] private Bullet _bulletPrefab;

        [SerializeField] private Transform _bulletLocation;
        [SerializeField] private Transform _bulletPivoit;

        private NetworkingPlayer _player;

        private void Start() => _player = GetComponent<NetworkingPlayer>();

        [Command]
        private void CMDShoot()
        {
            Bullet bullet = Instantiate(_bulletPrefab, _bulletLocation.position, _bulletPivoit.rotation);

            bullet.SetPlayerId(netId);
            bullet.SetPlayerId(_player.PlayerID);
            bullet.SetPlayerBulletDamage(_player.PlayerDamage);
            bullet.SetPlayerBulletSpeed(_player.PlayerBulletSpeed);

            RPCShootBullet(bullet.transform.position, bullet.transform.rotation);
        }

        [ClientRpc]
        private void RPCShootBullet(Vector3 _position, Quaternion _rotation)
        {
            //GameObject bullet = Instantiate(Resources.Load<GameObject>("Bullet"), _bulletLocation.position, _bulletLocation.rotation);
            //bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 10;

            if (!NetworkingManager.Singleton.IsHost)
            {
                Bullet bullet = Instantiate(_bulletPrefab.gameObject, _position, _rotation).GetComponent<Bullet>();
                bullet.SetPlayerId(netId);
                bullet.SetPlayerId(_player.PlayerID);
                bullet.SetPlayerBulletDamage(_player.PlayerDamage);
                bullet.SetPlayerBulletSpeed(_player.PlayerBulletSpeed);
            }
        }

        private void Update()
        {
            if (isLocalPlayer)
                if (Input.GetMouseButtonDown(0))
                    CMDShoot();
        }
    }
}
using UnityEngine;

namespace NGO_ToonTanks
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Transform _cannonPivot;

        private Rigidbody _rb;
        private NetworkingPlayer _player;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _player = GetComponent<NetworkingPlayer>();
        }

        private void Update()
        {
            if (_player.IsLocalPlayer && !_player.IsDead)
            {
                float horizontal = Input.GetAxis("Horizontal");
                float vertical = Input.GetAxis("Vertical");

                Vector3 direction = new(horizontal, 0, vertical);
                Vector3 velocity = direction * _player.PlayerSpeed.Value;

                _rb.velocity = velocity;

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane ground = new(Vector3.up, Vector3.zero);

                if (ground.Raycast(ray, out float distance))
                {
                    Vector3 point = ray.GetPoint(distance);
                    _cannonPivot.LookAt(new Vector3(point.x, _cannonPivot.position.y, point.z));
                }
            }
        }
    }
}
using Mirror;
using UnityEngine;

namespace Mirror_Tanks
{
    public class PlayerMovement : NetworkBehaviour
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
            if (isLocalPlayer && Application.isFocused)
            {
                //Movement
                float x = Input.GetAxis("Horizontal");
                float z = Input.GetAxis("Vertical");

                _rb.velocity = new Vector3(x, 0, z) * _player.PlayerMovementSpeed;
                //_rb.MovePosition(transform.position + _speed * Time.deltaTime * (transform.forward * z + transform.right * x));

                //Vector3 movement = _speed * Time.deltaTime * new Vector3(x, 0, z);
                //_rb.MovePosition(_rb.position + movement);

                //Cannon Rotation
                //bool right = Input.GetKey(KeyCode.Q);
                //bool left = Input.GetKey(KeyCode.E);

                //if (right || left)
                //    _cannonPivot.Rotate(Vector3.up, (right ? 1 : -1) * _rotationSpeed * Time.deltaTime);

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Vector3 targetPosition = hit.point;
                    targetPosition.y = _cannonPivot.position.y;
                    _cannonPivot.LookAt(targetPosition);
                }
            }
        }
    }
}
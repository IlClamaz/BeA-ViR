using UnityEngine;
using Mirror;
using Beavir.Businesslogic.Controllers;

namespace Beavir.Businesslogic.Utilities
{
    public class MoveAroundHemi : NetworkBehaviour
    {
        [SerializeField] private float verticalClampDelta;
        [SerializeField] private float distanceFromCamera;
        private float startPosition;
        private Transform playerCamera;
        private bool isMoving = true;

        public bool IsMoving { get => isMoving; set => isMoving = value; }

        void Start()
        {
            startPosition = transform.position.y;
        }

        [ServerCallback]
        void Update()
        {
            if (isMoving)
            {
                if (playerCamera == null) playerCamera = BeavirAppManager.Instance.UserManager.Player.PlayerCamera;
                //IN ORDER TO FACE ALWAYS CAMERA ON THE RIGHT SIDE
                Vector3 difference = playerCamera.position - transform.position;
                float rotationY = Mathf.Atan2(difference.x, difference.z) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0.0f, rotationY, 0.0f);

                //IN ORDER TO FOLLOW ALWAYS CAMERA ON A LIMITED EMISPHERE
                if (-0.6f <= playerCamera.forward.y && playerCamera.forward.y <= 0.6f)
                {
                    Vector3 positionNew = playerCamera.position + playerCamera.forward * distanceFromCamera;
                    positionNew.y = Mathf.Clamp(positionNew.y, startPosition - verticalClampDelta, startPosition + verticalClampDelta);
                    transform.position = positionNew;
                }
            }
        }
    }
}
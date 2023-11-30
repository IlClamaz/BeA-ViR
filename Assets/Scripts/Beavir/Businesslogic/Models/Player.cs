using UnityEngine;

namespace Beavir.Businesslogic.Models
{
    /// <summary>
    /// The virtual representation of the user, even called visitor. 
    /// </summary>
    public class Player : MonoBehaviour
    {
        [SerializeField] private Transform playerCamera = default;
        [SerializeField] private float speed = 11f;
        [SerializeField] private float jumpHeight = 3.5f;
        [SerializeField] private float gravity = -15f;
        [SerializeField] private bool fly = true;
        [SerializeField] private float sensitivityX = 0.1f;
        [SerializeField] private float sensitivityY = 0.1f;
        [SerializeField] private float yClamp = 35f;
        [SerializeField] private CharacterController characterController;
        private Vector2 cameraMovement;
        private Vector3 horizontalVelocity, verticalVelocity;

        public CharacterController CharacterController { get => characterController; }
        public Transform PlayerCamera { get => playerCamera; }

        public void RecalculateMovement(Vector2 nextMovement)
        {
            if (characterController.enabled)
            {
                horizontalVelocity = (transform.right * nextMovement.x + transform.forward * nextMovement.y) * speed;
                characterController.Move(horizontalVelocity * Time.deltaTime);

                verticalVelocity.y += gravity * Time.deltaTime;
                characterController.Move(verticalVelocity * Time.deltaTime);
            }
        }

        public void RotateCamera(Vector2 nextCameraRotation)
        {
            cameraMovement.y -= nextCameraRotation.y * sensitivityY * Time.deltaTime;
            cameraMovement.y = Mathf.Clamp(cameraMovement.y, -yClamp, yClamp);
            playerCamera.localRotation = Quaternion.Euler(cameraMovement.y, 0, 0); //Rotates camera up and down
            cameraMovement.x = nextCameraRotation.x * sensitivityX * Time.deltaTime;
            transform.Rotate(0, cameraMovement.x, 0); //Rotates camera and player around y axis
        }

        public void ChangePosition(Vector3 position)
        {
            DisableMovement();
            transform.position = position;
            EnableMovement();
        }

        public void ChangeRotation(Vector3 direction)
        {
            DisableMovement();
            transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            playerCamera.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            cameraMovement = Vector2.zero;
            EnableMovement();
        }

        public void Jump()
        {
            if (fly) verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
        }

        public void EnableMovement()
        {
            characterController.enabled = true;
        }

        public void DisableMovement()
        {
            characterController.enabled = false;
        }
    }
}

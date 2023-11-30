using Beavir.Businesslogic.Models;
using Beavir.Businesslogic.Utilities;
using UnityEngine;

namespace Beavir.Businesslogic.Controllers
{
    /// <summary>
    /// Controller that manages the User (virtualized through the Player) and devices input (through the Input Reader).
    /// </summary>
    public class UserManager : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;
        [SerializeField] private Player player;

        private bool controlsEnabled = true;
        private Vector2 nextMovement;
        private Vector2 nextCameraRotation;
        private bool isJumping;

        public InputReader InputReader { get => inputReader; }
        public Player Player { get => player; }
        public bool ControlsEnabled { get => controlsEnabled; }

        //Adds listeners for events being triggered in the InputReader script
        private void OnEnable()
        {
            inputReader.moveEvent += OnMove;
            inputReader.jumpEvent += OnJump;
            inputReader.cameraMoveEvent += OnRotateCamera;
        }

        //Removes all listeners to the events coming from the InputReader script
        private void OnDisable()
        {
            inputReader.moveEvent -= OnMove;
            inputReader.jumpEvent -= OnJump;
            inputReader.cameraMoveEvent -= OnRotateCamera;
        }

        private void OnMove(Vector2 movement)
        {
            nextMovement = movement;
        }

        private void OnRotateCamera(Vector2 rotation)
        {
            nextCameraRotation = rotation;
        }

        private void OnJump()
        {
            isJumping = true;
        }

        public void Start()
        {
            /* if (BeavirAppManager.Instance.TransportManager.CurEnv.IsMainHall)
                UpdatePlayerPosition(BeavirAppManager.Instance.TransportManager.StartingPoint.transform.position);
            else
                UpdatePlayerPosition(new Vector3(-2, 0.07f, 17)); */
        }

        private void LateUpdate()
        {
            if (player.isActiveAndEnabled) player.RotateCamera(nextCameraRotation);
        }

        private void Update()
        {
            if (player.isActiveAndEnabled) player.RecalculateMovement(nextMovement);
            if (isJumping)
            {
                player.Jump();
                isJumping = false;
            }
        }

        public void DisableAllInput()
        {
            if (controlsEnabled)
            {
                inputReader.DisableAllInput();
                controlsEnabled = false;
            }
        }

        public void TemporarilyDisableAllInput(float duration)
        {
            if (controlsEnabled)
            {
                inputReader.DisableAllInput();
                Invoke(nameof(EnableAllInput), duration + 0.1f);
                controlsEnabled = false;
            }
        }

        public void EnableAllInput()
        {
            if (!controlsEnabled)
            {
                inputReader.EnableGameplayInput();
                controlsEnabled = true;
            }
        }

        public void UpdatePlayerPosition(Vector3 destination)
        {
            player.ChangePosition(destination);
        }

        public void UpdatePlayerRotation(Vector3 direction)
        {
            player.ChangeRotation(direction);
        }
    }
}

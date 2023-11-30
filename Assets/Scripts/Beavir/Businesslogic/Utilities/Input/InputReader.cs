using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace Beavir.Businesslogic.Utilities
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Game/Input Reader")]
    public class InputReader : ScriptableObject, GameInput.IGameplayActions
    {
        // Assign delegate{} to events to initialise them with an empty delegate
        // so we can skip the null check when we use them

        // Gameplay
        public event UnityAction jumpEvent = delegate { };
        public event UnityAction jumpCanceledEvent = delegate { };
        public event UnityAction showOverlayEvent = delegate { }; // Used to bring up the overlay
        public event UnityAction showCommandsEvent = delegate { }; // Used to bring up the commands overlay
        public event UnityAction audioEnablerEvent = delegate { }; // Used to bring up the music sources
        public event UnityAction goBackHomeEvent = delegate { }; // Used to go back home
        public event UnityAction pauseEvent = delegate { };
        public event UnityAction<Vector2> moveEvent = delegate { };
        public event UnityAction<Vector2> cameraMoveEvent = delegate { };
        public event UnityAction<float> sightSeeingEvent = delegate { };
        public event UnityAction startedRunning = delegate { };
        public event UnityAction stoppedRunning = delegate { };
        public event UnityAction resetEvent = delegate { };
        public event UnityAction itemPauseResumeEvent = delegate { };


        private GameInput gameInput;

        private void OnEnable()
        {
            if (gameInput == null)
            {
                gameInput = new GameInput();
                gameInput.Gameplay.SetCallbacks(this);
            }

            EnableGameplayInput();
        }

        private void OnDisable()
        {
            DisableAllInput();
        }

        public void OnShowOverlay(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) showOverlayEvent.Invoke();

            if (gameInput.Gameplay.AudioEnabler.enabled) gameInput.Gameplay.AudioEnabler.Disable();
            else gameInput.Gameplay.AudioEnabler.Enable();
            if (gameInput.Gameplay.GoBackHome.enabled) gameInput.Gameplay.GoBackHome.Disable();
            else gameInput.Gameplay.GoBackHome.Enable();
            if (gameInput.Gameplay.ItemRotation.enabled) gameInput.Gameplay.ItemRotation.Disable();
            else gameInput.Gameplay.ItemRotation.Enable();
            if (gameInput.Gameplay.RotateCamera.enabled) gameInput.Gameplay.RotateCamera.Disable();
            else gameInput.Gameplay.RotateCamera.Enable();
            if (gameInput.Gameplay.Move.enabled) gameInput.Gameplay.Move.Disable();
            else gameInput.Gameplay.Move.Enable();
            if (gameInput.Gameplay.ShowCommands.enabled) gameInput.Gameplay.ShowCommands.Disable();
            else gameInput.Gameplay.ShowCommands.Enable();
            if (gameInput.Gameplay.Jump.enabled) gameInput.Gameplay.Jump.Disable();
            else gameInput.Gameplay.Jump.Enable();
            if (gameInput.Gameplay.SightSeeing.enabled) gameInput.Gameplay.SightSeeing.Disable();
            else gameInput.Gameplay.SightSeeing.Enable();
        }

        public void OnShowCommands(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) showCommandsEvent.Invoke();

            if (gameInput.Gameplay.AudioEnabler.enabled) gameInput.Gameplay.AudioEnabler.Disable();
            else gameInput.Gameplay.AudioEnabler.Enable();
            if (gameInput.Gameplay.GoBackHome.enabled) gameInput.Gameplay.GoBackHome.Disable();
            else gameInput.Gameplay.GoBackHome.Enable();
            if (gameInput.Gameplay.ItemRotation.enabled) gameInput.Gameplay.ItemRotation.Disable();
            else gameInput.Gameplay.ItemRotation.Enable();
            if (gameInput.Gameplay.RotateCamera.enabled) gameInput.Gameplay.RotateCamera.Disable();
            else gameInput.Gameplay.RotateCamera.Enable();
            if (gameInput.Gameplay.Move.enabled) gameInput.Gameplay.Move.Disable();
            else gameInput.Gameplay.Move.Enable();
            if (gameInput.Gameplay.ShowOverlay.enabled) gameInput.Gameplay.ShowOverlay.Disable();
            else gameInput.Gameplay.ShowOverlay.Enable();
            if (gameInput.Gameplay.Jump.enabled) gameInput.Gameplay.Jump.Disable();
            else gameInput.Gameplay.Jump.Enable();
            if (gameInput.Gameplay.SightSeeing.enabled) gameInput.Gameplay.SightSeeing.Disable();
            else gameInput.Gameplay.SightSeeing.Enable();
        }

        public void OnAudioEnabler(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) audioEnablerEvent.Invoke();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
                jumpEvent.Invoke();

            if (context.phase == InputActionPhase.Canceled)
                jumpCanceledEvent.Invoke();
        }

        public void OnReset(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) resetEvent.Invoke();
        }

        public void OnItemRotation(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) itemPauseResumeEvent.Invoke();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            moveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnSightSeeing(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) sightSeeingEvent.Invoke(context.ReadValue<float>());
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Performed:
                    startedRunning.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    stoppedRunning.Invoke();
                    break;
            }
        }

        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) pauseEvent.Invoke();
        }

        public void OnRotateCamera(InputAction.CallbackContext context)
        {
            cameraMoveEvent.Invoke(context.ReadValue<Vector2>());
        }

        public void OnGoBackHome(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed) goBackHomeEvent.Invoke();
        }

        public void EnableGameplayInput()
        {
            gameInput.Gameplay.Enable();
        }

        public void DisableAllInput()
        {
            gameInput.Gameplay.Disable();
        }
    }

}

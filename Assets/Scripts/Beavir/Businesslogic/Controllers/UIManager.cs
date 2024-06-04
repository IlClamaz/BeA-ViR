using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using Beavir.Businesslogic.Utilities;

namespace Beavir.Businesslogic.Controllers
{
    /// <summary>
    /// Controller that manages UIs (Overlays, CommandsUI, Title UIs, Loading, Tooltip, Screensaver) 
    /// </summary>
    public class UIManager : NetworkBehaviour
    {
        [SerializeField] private GameObject commandsUI;
        [SerializeField] private GameObject noInternetUI;
        [SerializeField] private GameObject loadingUI;
        [SerializeField] private GameObject tooltipUI;
        [SerializeField] private Image progressBar;
        [SerializeField] private bool useScreenSaver = true;

        [DrawIf("useScreenSaver", true)]
        [SerializeField] private GameObject screenSaver;

        [DrawIf("useScreenSaver", true)]
        [SerializeField] private int idleTimeSetting;

        private float lastIdleTime;
        private IDisposable buttonPressListener;
        private Vector2 movement, newCameraMovement;
        private bool overlayIsOn, commandsUIIsOn, loadingUIIsOn, tooltipUIIsOn;

        public Image ProgressBar { get => progressBar; }
        public GameObject NoInternetUI { get => noInternetUI; }

        private void OnEnable()
        {
            BeavirAppManager.Instance.UserManager.InputReader.showCommandsEvent += ShowCommandsUI;
            BeavirAppManager.Instance.UserManager.InputReader.showOverlayEvent += ShowOverlay;
            BeavirAppManager.Instance.UserManager.InputReader.moveEvent += OnMove;
            BeavirAppManager.Instance.UserManager.InputReader.cameraMoveEvent += OnRotateCamera;
        }

        private void OnDisable()
        {
            BeavirAppManager.Instance.UserManager.InputReader.showCommandsEvent -= ShowCommandsUI;
            BeavirAppManager.Instance.UserManager.InputReader.showOverlayEvent -= ShowOverlay;
            BeavirAppManager.Instance.UserManager.InputReader.moveEvent -= OnMove;
            BeavirAppManager.Instance.UserManager.InputReader.cameraMoveEvent -= OnRotateCamera;
        }


        void Start()
        {
            overlayIsOn = false;
            commandsUIIsOn = false;
            tooltipUIIsOn = false;
            loadingUIIsOn = true;

            if (useScreenSaver)
            {
                lastIdleTime = Time.time;
                buttonPressListener = InputSystem.onAnyButtonPress.Call(button => lastIdleTime = Time.time);
            }
        }

        void Update()
        {
            if (BeavirAppManager.Instance.UserManager.ControlsEnabled && useScreenSaver)
            {
                if (movement != Vector2.zero || newCameraMovement != Vector2.zero) lastIdleTime = Time.time;
                if (IdleCheck())
                {
                    if (!screenSaver.activeSelf) ObjectEnabler.Instance.ActivateGameObject(screenSaver, 0);
                }
                else
                {
                    if (screenSaver.activeSelf)
                    {
                        NetworkManager.singleton.StopHost();
                        NetworkManager.singleton.ServerChangeScene(SceneManager.GetActiveScene().name);
                    }
                }
            }
        }

        private void OnMove(Vector2 _movement)
        {
            movement = _movement;
        }

        private void OnRotateCamera(Vector2 _movement)
        {
            newCameraMovement = _movement;
        }

        private bool IdleCheck()
        {
            return Time.time - lastIdleTime > idleTimeSetting;
        }

        [ServerCallback]
        public void ShowTitleUI()
        {
            BeavirAppManager.Instance.TransportManager.CurEnv.ShowTitleUI();
        }

        private void ShowOverlay()
        {
            ShowTooltipUI();
            overlayIsOn = !overlayIsOn;
            BeavirAppManager.Instance.TransportManager.CurEnv.ShowOverlay(overlayIsOn);
        }

        private void ShowCommandsUI()
        {
            ShowTooltipUI();
            commandsUIIsOn = !commandsUIIsOn;
            if (commandsUIIsOn) ObjectEnabler.Instance.ActivateGameObject(commandsUI, 0);
            else ObjectEnabler.Instance.DeactivateGameObject(commandsUI, 0);
        }

        public void ShowLoadingUI()
        {
            loadingUIIsOn = !loadingUIIsOn;
            if (loadingUIIsOn) ObjectEnabler.Instance.ActivateGameObject(loadingUI, 0);
            else ObjectEnabler.Instance.DeactivateGameObject(loadingUI, 0);
        }

        public void ShowTooltipUI()
        {
            tooltipUIIsOn = !tooltipUIIsOn;
            if (tooltipUIIsOn) ObjectEnabler.Instance.ActivateGameObject(tooltipUI, 0);
            else ObjectEnabler.Instance.DeactivateGameObject(tooltipUI, 0);
        }

        void OnDestroy()
        {
            if (useScreenSaver) buttonPressListener.Dispose();
        }
    }
}

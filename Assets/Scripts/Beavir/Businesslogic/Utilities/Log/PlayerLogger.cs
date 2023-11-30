using Beavir.Businesslogic.Controllers;
using UnityEngine;


namespace Beavir.Businesslogic.Utilities
{
    public class PlayerLogger : MonoBehaviour
    {
        private int overlayCount, commandsCount, audioCount, jumpCount, sightCount, goBackCount = 0;
        [SerializeField] private InputReader inputReader = default;
        private void OnEnable()
        {
            inputReader.showOverlayEvent += OnShowOverlay;
            inputReader.showCommandsEvent += OnShowCommands;
            inputReader.audioEnablerEvent += OnAudioEnabler;
            inputReader.jumpEvent += OnJump;
            inputReader.sightSeeingEvent += OnSightSeeing;
            inputReader.goBackHomeEvent += OnGoBackHome;

        }

        private void OnDisable()
        {
            inputReader.showOverlayEvent -= OnShowOverlay;
            inputReader.showCommandsEvent -= OnShowCommands;
            inputReader.audioEnablerEvent -= OnAudioEnabler;
            inputReader.jumpEvent -= OnJump;
            inputReader.sightSeeingEvent -= OnSightSeeing;
            inputReader.goBackHomeEvent -= OnGoBackHome;
            Debug.Log("-- COMMANDS --" + "\n" +
            "overlay;" + overlayCount + "\n" +
            "commands;" + commandsCount + "\n" +
            "audio;" + audioCount + "\n" +
            "jump;" + jumpCount + "\n" +
            "sight;" + sightCount + "\n" +
            "goback;" + goBackCount);

            Debug.Log("Location History:");
            foreach (var item in BeavirAppManager.Instance.TransportManager.LocHistory)
            {
                Debug.Log(item.name);
            }

        }

        private void OnShowOverlay()
        {
            overlayCount++;
        }

        private void OnShowCommands()
        {
            commandsCount++;
        }

        private void OnAudioEnabler()
        {
            audioCount++;
        }

        private void OnJump()
        {
            jumpCount++;
        }

        private void OnSightSeeing(float key)
        {
            sightCount++;
        }

        private void OnGoBackHome()
        {
            goBackCount++;
        }
    }
}

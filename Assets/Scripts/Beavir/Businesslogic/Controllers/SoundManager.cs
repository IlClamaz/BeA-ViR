using UnityEngine;
using Mirror;

namespace Beavir.Businesslogic.Controllers
{
    /// <summary>
    /// Controller that manages audio play/stop requests (both soundtrack and single sounds)
    /// </summary>
    public class SoundManager : NetworkBehaviour
    {
        private bool audioIsOn;
        public bool AudioIsOn { get => audioIsOn; set => audioIsOn = value; }
        private void OnEnable()
        {
            BeavirAppManager.Instance.UserManager.InputReader.audioEnablerEvent += ManageAudio;
        }

        private void OnDisable()
        {
            BeavirAppManager.Instance.UserManager.InputReader.audioEnablerEvent -= ManageAudio;
        }

        private void Start()
        {
            audioIsOn = false;
            BeavirAppManager.Instance.TransportManager.CurEnv.ManageAudio(audioIsOn);
            ManageAudio();
        }

        /// <summary>
        /// Method that changes the Environment soundtrack reproduction
        /// </summary>
        [ServerCallback]
        private void ManageAudio()
        {
            audioIsOn = !audioIsOn;
            BeavirAppManager.Instance.TransportManager.CurEnv.ManageAudio(audioIsOn);
        }

        /// <summary>
        /// Method that plays a single sound (a clip in a <paramref name="audioSource"/>) in
        /// a specific position <paramref name="position"/>
        /// </summary>
        [ServerCallback]
        public void PlayAudio(AudioSource audioSource, Vector3 position)
        {
            AudioSource.PlayClipAtPoint(audioSource.clip, position);
        }
    }
}

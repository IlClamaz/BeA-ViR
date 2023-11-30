using UnityEngine;
using UnityEngine.Video;

namespace Beavir.Businesslogic.Utilities
{
    public class VideoPlayerRestart : MonoBehaviour
    {
        private AudioSource[] audioSources;
        // Start is called before the first frame update
        void Awake()
        {
            gameObject.GetComponent<VideoPlayer>().targetTexture.Release();
            audioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
            foreach (var source in audioSources)
            {
                source.Stop();
            }
        }
    }
}

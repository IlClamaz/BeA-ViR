using Beavir.Businesslogic.Controllers;
using Beavir.Businesslogic.Models;
using UnityEngine;

namespace Beavir.Businesslogic.View
{
    /// <summary>
    /// View that handles the collision with the player, delegating the controllers
    /// </summary>
    public class E2EObj : MonoBehaviour
    {
        [SerializeField] private Environment destinationEnvironment;

        public Environment DestinationEnvironment { get => destinationEnvironment; }

        public void OnTriggerEnter(Collider other)
        {
            if (TryGetComponent(out AudioSource audioSource))
            {
                if (destinationEnvironment.IsMainHall)
                    BeavirAppManager.Instance.SoundManager.PlayAudio(audioSource, BeavirAppManager.Instance.TransportManager.StartingPoint.transform.position);
                else
                    BeavirAppManager.Instance.SoundManager.PlayAudio(audioSource, BeavirAppManager.Instance.UserManager.Player.transform.position);
            }

            BeavirAppManager.Instance.TransportManager.TravelTo(destinationEnvironment);
        }
    }
}

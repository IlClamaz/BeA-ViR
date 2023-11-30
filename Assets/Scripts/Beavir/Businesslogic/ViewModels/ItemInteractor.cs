using Beavir.Businesslogic.Controllers;
using Mirror;
using UnityEngine;

namespace Beavir.Businesslogic.ViewModels
{
    /// <summary>
    /// Base class for handling Current Object updates after specific player actions. 
    /// </summary>
    public class ItemInteractor : NetworkBehaviour
    {
        public GameObject item;
        public void OnTriggerEnter(Collider other)
        {
            BeavirAppManager.Instance.TransportManager.CurObj = item.transform.GetChild(0).gameObject;
            BeavirAppManager.Instance.TransportManager.UpdateLocHistory(item.transform.GetChild(0).gameObject);
        }

        public void OnTriggerExit(Collider other)
        {
            BeavirAppManager.Instance.TransportManager.CurObj = null;
        }
    }
}

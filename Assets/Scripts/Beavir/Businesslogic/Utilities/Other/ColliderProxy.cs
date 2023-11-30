using UnityEngine;
using System;

namespace Beavir.Businesslogic.Utilities
{
    public class ColliderProxy : MonoBehaviour
    {

        public Action<Collider> OnTriggerEnter_Action;
        public Action<Collider> OnTriggerExit_Action;

        private void OnTriggerEnter(Collider other)
        {
            //print("collisione per " + gameObject.name + " by " + other.gameObject.name);
            OnTriggerEnter_Action?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExit_Action?.Invoke(other);
        }
    }
}
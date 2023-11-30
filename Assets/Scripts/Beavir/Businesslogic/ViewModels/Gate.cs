using Beavir.Businesslogic.Utilities;
using Beavir.Businesslogic.View;
using System.Collections.Generic;
using UnityEngine;

namespace Beavir.Businesslogic.ViewModels
{
    /// <summary>
    /// Contains the methods that handle the Gate animation (topology map and light beam)
    /// Contains a reference to E2EObj that leads to the destination Environment. 
    /// </summary>
    public class Gate : MonoBehaviour
    {
        [SerializeField] private List<GameObject> mapElementsToDisable, footboards;
        [SerializeField] private GameObject beam;
        [SerializeField] private List<Animator> mapElementsAnimators;
        [SerializeField] private ColliderProxy geoTrigger, gateModel;

        private E2EObj e2EObj;
        private bool swapped = false;

        public GameObject Beam { get => beam; set => beam = value; }

        // Start is called before the first frame update
        void Start()
        {
            e2EObj = GetComponent<E2EObj>();
            geoTrigger.OnTriggerEnter_Action += (c) =>
            {
                ObjectEnabler.Instance.ActivateGameObject(beam, 0);
                ObjectEnabler.Instance.DeactivateGameObject(mapElementsToDisable, 0);
                ObjectAnimator.Instance.PlayAnimation(mapElementsAnimators, "ZoomTrigger");
            };

            geoTrigger.OnTriggerExit_Action += (c) =>
            {
                ObjectEnabler.Instance.DeactivateGameObject(beam, 0);
                ObjectEnabler.Instance.ActivateGameObject(mapElementsToDisable, 0);
                ObjectAnimator.Instance.StopAnimation(mapElementsAnimators, "ZoomTrigger");
            };

            gateModel.OnTriggerEnter_Action += EnterGate;
        }

        private void EnterGate(Collider c)
        {
            Reset();
            if (!swapped)
            {
                ObjectEnabler.Instance.ToggleGameObject(footboards, 0);
                swapped = true;
            }
            e2EObj.OnTriggerEnter(c);
        }

        public void Reset()
        {
            ObjectEnabler.Instance.DeactivateGameObject(beam, 0);
            ObjectEnabler.Instance.ActivateGameObject(mapElementsToDisable, 0);
            ObjectAnimator.Instance.ResetAnimator(mapElementsAnimators);
        }
    }
}

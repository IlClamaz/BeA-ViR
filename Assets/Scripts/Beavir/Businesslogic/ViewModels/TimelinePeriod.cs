using Beavir.Businesslogic.Controllers;
using Beavir.Businesslogic.Utilities;
using Beavir.Businesslogic.View;
using System.Collections.Generic;
using UnityEngine;

namespace Beavir.Businesslogic.ViewModels
{
    /// <summary>
    /// Handles the enablement of the gates related to a TimelinePeriod and the date/name animation. 
    /// </summary>
    public class TimelinePeriod : MonoBehaviour
    {
        private Animator periodAnimator;
        private E2EObj e2EObj;
        [SerializeField] private List<Gate> periodGates;
        [SerializeField] private ColliderProxy periodTrigger;

        private void Start()
        {
            periodAnimator = GetComponent<Animator>();
            e2EObj = GetComponent<E2EObj>();
            periodTrigger.OnTriggerEnter_Action += EnterPeriod;
            periodTrigger.OnTriggerExit_Action += (Collider c) => ObjectAnimator.Instance.StopAnimation(periodAnimator, "ZoomColorTrigger");
        }

        private void EnterPeriod(Collider c)
        {
            e2EObj.OnTriggerEnter(null);
            ObjectAnimator.Instance.PlayAnimation(periodAnimator, "ZoomColorTrigger");
            foreach (var gate in periodGates)
                ObjectEnabler.Instance.ActivateGameObject(gate.gameObject, BeavirAppManager.Instance.TransportManager.FadingDuration + 0.1f);
        }

        public void Reset()
        {
            ObjectAnimator.Instance.ResetAnimator(periodAnimator);
        }
    }
}
